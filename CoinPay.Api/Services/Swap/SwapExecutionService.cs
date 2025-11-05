using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using CoinPay.Api.Services.Swap.Exceptions;
using CoinPay.Api.Services.Swap.OneInch;
using CoinPay.Api.Services.Wallet;
using Microsoft.Extensions.Logging;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Core service for executing token swaps via DEX aggregator
/// </summary>
public class SwapExecutionService : ISwapExecutionService
{
    private readonly IDexAggregatorService _dexService;
    private readonly IWalletService _walletService;
    private readonly ITokenBalanceValidationService _balanceService;
    private readonly ISlippageToleranceService _slippageService;
    private readonly ISwapTransactionRepository _swapRepository;
    private readonly IFeeCalculationService _feeService;
    private readonly IPlatformFeeCollectionService _feeCollectionService;
    private readonly ILogger<SwapExecutionService> _logger;

    public SwapExecutionService(
        IDexAggregatorService dexService,
        IWalletService walletService,
        ITokenBalanceValidationService balanceService,
        ISlippageToleranceService slippageService,
        ISwapTransactionRepository swapRepository,
        IFeeCalculationService feeService,
        IPlatformFeeCollectionService feeCollectionService,
        ILogger<SwapExecutionService> logger)
    {
        _dexService = dexService;
        _walletService = walletService;
        _balanceService = balanceService;
        _slippageService = slippageService;
        _swapRepository = swapRepository;
        _feeService = feeService;
        _feeCollectionService = feeCollectionService;
        _logger = logger;
    }

    public async Task<SwapExecutionResult> ExecuteSwapAsync(
        Guid userId,
        string walletAddress,
        string fromToken,
        string toToken,
        decimal fromAmount,
        decimal slippageTolerance)
    {
        _logger.LogInformation(
            "Executing swap for user {UserId}: {FromAmount} {FromToken} -> {ToToken}, Slippage: {Slippage}%",
            userId,
            fromAmount,
            fromToken,
            toToken,
            slippageTolerance);

        try
        {
            // 1. Validate balance
            _logger.LogDebug("Step 1: Validating balance");
            var balanceCheck = await _balanceService.ValidateBalanceAsync(
                userId,
                walletAddress,
                fromToken,
                fromAmount);

            if (!balanceCheck.HasSufficientBalance)
            {
                throw new InsufficientBalanceException(
                    $"Insufficient balance. Required: {balanceCheck.TotalRequired}, Available: {balanceCheck.CurrentBalance}",
                    balanceCheck.TotalRequired,
                    balanceCheck.CurrentBalance);
            }

            // 2. Get swap transaction data from DEX
            _logger.LogDebug("Step 2: Getting swap transaction from DEX aggregator");
            var swapTx = await _dexService.GetSwapTransactionAsync(
                fromToken,
                toToken,
                fromAmount,
                slippageTolerance,
                walletAddress);

            // 3. Calculate fees and minimum received
            _logger.LogDebug("Step 3: Calculating fees");
            var platformFee = await _feeService.CalculateSwapFeeAsync(fromToken, fromAmount);
            var feePercentage = await _feeService.GetFeePercentageAsync(userId);
            var minimumReceived = _slippageService.CalculateMinimumReceived(
                swapTx.ExchangeRate * fromAmount,
                slippageTolerance);

            // Update swap transaction with fee info
            swapTx.PlatformFee = platformFee;
            swapTx.MinimumReceived = minimumReceived;

            // 4. Check token approval (if needed for ERC20 tokens)
            _logger.LogDebug("Step 4: Checking token approval");
            var needsApproval = await CheckTokenApprovalAsync(
                walletAddress,
                fromToken,
                swapTx.SpenderAddress,
                fromAmount);

            if (needsApproval)
            {
                _logger.LogInformation("Token approval required for spender: {Spender}", swapTx.SpenderAddress);
                // In production, execute approval transaction first
                // For MVP, we'll assume approval is handled externally or mock it
            }

            // 5. Create swap record in database
            _logger.LogDebug("Step 5: Creating swap record");
            var swapRecord = new SwapTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                WalletAddress = walletAddress,
                FromToken = fromToken,
                FromTokenSymbol = TestnetTokens.GetSymbol(fromToken),
                ToToken = toToken,
                ToTokenSymbol = TestnetTokens.GetSymbol(toToken),
                FromAmount = fromAmount,
                ToAmount = swapTx.ExchangeRate * fromAmount,
                ExchangeRate = swapTx.ExchangeRate,
                PlatformFee = platformFee,
                PlatformFeePercentage = feePercentage,
                SlippageTolerance = slippageTolerance,
                MinimumReceived = minimumReceived,
                DexProvider = "1inch",
                Status = SwapStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _swapRepository.CreateAsync(swapRecord);

            // 6. Execute swap transaction
            _logger.LogDebug("Step 6: Submitting swap transaction to blockchain");
            string? transactionHash = null;

            try
            {
                // For MVP with mock mode, we'll generate a mock transaction hash
                // In production, this would submit via Circle SDK or Web3 provider
                transactionHash = await SubmitSwapTransactionAsync(swapTx);

                swapRecord.TransactionHash = transactionHash;
                await _swapRepository.UpdateAsync(swapRecord);

                _logger.LogInformation(
                    "Swap transaction submitted: SwapId={SwapId}, TxHash={TxHash}",
                    swapRecord.Id,
                    transactionHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to submit swap transaction");
                swapRecord.Status = SwapStatus.Failed;
                swapRecord.ErrorMessage = ex.Message;
                await _swapRepository.UpdateAsync(swapRecord);
                throw;
            }

            // 7. Collect platform fee (async, non-blocking)
            _ = Task.Run(async () =>
            {
                await _feeCollectionService.CollectSwapFeeAsync(
                    userId,
                    swapRecord.Id,
                    platformFee);
            });

            return new SwapExecutionResult
            {
                SwapId = swapRecord.Id,
                TransactionHash = transactionHash,
                Status = SwapStatus.Pending,
                ExpectedToAmount = swapRecord.ToAmount,
                MinimumReceived = swapRecord.MinimumReceived,
                PlatformFee = swapRecord.PlatformFee,
                Message = "Swap transaction submitted successfully. Awaiting blockchain confirmation."
            };
        }
        catch (InsufficientBalanceException)
        {
            throw; // Re-throw to be handled by controller
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Swap execution failed for user {UserId}: {FromToken} -> {ToToken}",
                userId,
                fromToken,
                toToken);
            throw;
        }
    }

    private async Task<bool> CheckTokenApprovalAsync(
        string walletAddress,
        string tokenAddress,
        string spenderAddress,
        decimal amount)
    {
        // For MVP, skip actual approval check
        // In production, query token allowance on-chain:
        // 1. Call ERC20.allowance(walletAddress, spenderAddress)
        // 2. Compare with required amount
        // 3. Return true if allowance < amount

        _logger.LogDebug(
            "Checking token approval: Wallet={Wallet}, Token={Token}, Spender={Spender}",
            walletAddress,
            tokenAddress,
            spenderAddress);

        await Task.CompletedTask;
        return false; // Assume approval exists for MVP
    }

    private async Task<string> SubmitSwapTransactionAsync(DexSwapTransaction swapTx)
    {
        // For MVP, generate mock transaction hash
        // In production, submit via Circle SDK or Web3 provider:
        // 1. Sign transaction with user's wallet
        // 2. Submit to blockchain
        // 3. Return transaction hash

        _logger.LogInformation(
            "Submitting swap transaction (MOCK MODE): To={To}, Data={Data}",
            swapTx.To,
            swapTx.Data.Substring(0, Math.Min(20, swapTx.Data.Length)) + "...");

        await Task.Delay(100); // Simulate network delay

        // Generate mock transaction hash
        var mockTxHash = "0x" + Guid.NewGuid().ToString("N") + DateTime.UtcNow.Ticks.ToString("x");

        return mockTxHash;
    }
}
