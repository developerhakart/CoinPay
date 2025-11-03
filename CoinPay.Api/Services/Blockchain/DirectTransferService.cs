using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Util;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;

namespace CoinPay.Api.Services.Blockchain;

/// <summary>
/// Direct blockchain transfer service using Nethereum
/// WARNING: For testing/development only. Requires private key management.
/// Production should use Circle's custody solution.
/// </summary>
public class DirectTransferService : IDirectTransferService
{
    private readonly ILogger<DirectTransferService> _logger;
    private readonly Web3 _web3;
    private readonly Account _account;
    private readonly string _usdcContractAddress;
    private const string POLYGON_AMOY_RPC = "https://rpc-amoy.polygon.technology";
    private const string USDC_CONTRACT = "0x41E94Eb019C0762f9Bfcf9Fb1E58725BfB0e7582";

    public DirectTransferService(IConfiguration configuration, ILogger<DirectTransferService> logger)
    {
        _logger = logger;
        _usdcContractAddress = USDC_CONTRACT;

        // Get private key from configuration
        var privateKey = Environment.GetEnvironmentVariable("TEST_WALLET_PRIVATE_KEY")
                        ?? configuration["Blockchain:TestWallet:PrivateKey"];

        if (string.IsNullOrEmpty(privateKey))
        {
            _logger.LogWarning("No test wallet private key configured. Direct transfers will not work.");
            _logger.LogWarning("Set TEST_WALLET_PRIVATE_KEY environment variable or Blockchain:TestWallet:PrivateKey in appsettings.");
            throw new InvalidOperationException("Test wallet private key not configured");
        }

        // Ensure private key starts with 0x
        if (!privateKey.StartsWith("0x"))
        {
            privateKey = "0x" + privateKey;
        }

        try
        {
            _account = new Account(privateKey);
            _web3 = new Web3(_account, POLYGON_AMOY_RPC);
            _logger.LogInformation("DirectTransferService initialized. Test wallet: {Address}", _account.Address);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize DirectTransferService with provided private key");
            throw new InvalidOperationException("Invalid private key configuration", ex);
        }
    }

    public string GetTestWalletAddress()
    {
        return _account.Address;
    }

    public async Task<DirectTransferResult> SendNativeAsync(
        string toAddress,
        decimal amountInMatic,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending {Amount} POL from {From} to {To}",
            amountInMatic, _account.Address, toAddress);

        try
        {
            // Validate addresses
            if (!Web3.IsChecksumAddress(toAddress) && !toAddress.StartsWith("0x"))
            {
                throw new ArgumentException("Invalid recipient address format", nameof(toAddress));
            }

            // Convert amount to Wei (1 MATIC = 10^18 Wei)
            var amountInWei = Web3.Convert.ToWei(amountInMatic);

            // Get current gas price
            var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();

            // Estimate gas (standard transfer is ~21000)
            var gasLimit = new HexBigInteger(21000);

            // Create transaction input
            var transactionInput = new TransactionInput
            {
                From = _account.Address,
                To = toAddress,
                Value = new HexBigInteger(amountInWei),
                Gas = gasLimit,
                GasPrice = gasPrice
            };

            // Send transaction
            _logger.LogInformation("Sending transaction with gas price: {GasPrice} Wei, gas limit: {GasLimit}",
                gasPrice.Value, gasLimit.Value);

            var txHash = await _web3.Eth.TransactionManager.SendTransactionAsync(transactionInput);

            _logger.LogInformation("Transaction sent successfully. TxHash: {TxHash}", txHash);

            // Wait for transaction receipt (with timeout)
            var receipt = await WaitForTransactionReceiptAsync(txHash, cancellationToken);

            var gasUsed = receipt?.GasUsed?.Value ?? BigInteger.Zero;
            var gasCost = BigInteger.Multiply(gasUsed, gasPrice.Value);
            var gasUsedInMatic = Web3.Convert.FromWei(gasCost);

            return new DirectTransferResult
            {
                TxHash = txHash,
                FromAddress = _account.Address,
                ToAddress = toAddress,
                Amount = amountInMatic,
                Currency = "POL",
                Status = receipt?.Status?.Value == 1 ? "Confirmed" : "Failed",
                GasUsed = gasUsedInMatic,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send POL transaction to {ToAddress}", toAddress);
            throw new InvalidOperationException($"Failed to send POL: {ex.Message}", ex);
        }
    }

    public async Task<DirectTransferResult> SendUsdcAsync(
        string toAddress,
        decimal amountInUsdc,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending {Amount} USDC from {From} to {To}",
            amountInUsdc, _account.Address, toAddress);

        try
        {
            // Validate addresses
            if (!Web3.IsChecksumAddress(toAddress) && !toAddress.StartsWith("0x"))
            {
                throw new ArgumentException("Invalid recipient address format", nameof(toAddress));
            }

            // USDC has 6 decimals
            var amountInSmallestUnit = (BigInteger)(amountInUsdc * 1_000_000m);

            // Get ERC20 contract service
            var tokenService = _web3.Eth.ERC20.GetContractService(_usdcContractAddress);

            // Get current balance to verify sufficient funds
            var balance = await tokenService.BalanceOfQueryAsync(_account.Address);
            var balanceInUsdc = (decimal)balance / 1_000_000m;
            _logger.LogInformation("Current USDC balance: {Balance}", balanceInUsdc);

            if (balance < amountInSmallestUnit)
            {
                throw new InvalidOperationException(
                    $"Insufficient USDC balance. Required: {amountInUsdc}, Available: {balanceInUsdc}");
            }

            // Send transfer transaction and wait for receipt
            var receipt = await tokenService.TransferRequestAndWaitForReceiptAsync(
                toAddress,
                amountInSmallestUnit);

            _logger.LogInformation("USDC transaction confirmed. TxHash: {TxHash}", receipt.TransactionHash);

            var gasUsed = receipt.GasUsed?.Value ?? BigInteger.Zero;
            var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
            var gasCost = BigInteger.Multiply(gasUsed, gasPrice.Value);
            var gasUsedInMatic = Web3.Convert.FromWei(gasCost);

            return new DirectTransferResult
            {
                TxHash = receipt.TransactionHash,
                FromAddress = _account.Address,
                ToAddress = toAddress,
                Amount = amountInUsdc,
                Currency = "USDC",
                Status = receipt.Status?.Value == 1 ? "Confirmed" : "Failed",
                GasUsed = gasUsedInMatic,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send USDC transaction to {ToAddress}", toAddress);
            throw new InvalidOperationException($"Failed to send USDC: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Wait for transaction receipt with timeout
    /// </summary>
    private async Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt?> WaitForTransactionReceiptAsync(
        string txHash,
        CancellationToken cancellationToken)
    {
        const int maxAttempts = 30; // 30 attempts
        const int delayMs = 2000;   // 2 seconds between attempts

        for (int i = 0; i < maxAttempts; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
            if (receipt != null)
            {
                _logger.LogInformation("Transaction receipt received after {Attempts} attempts", i + 1);
                return receipt;
            }

            _logger.LogDebug("Waiting for transaction receipt... Attempt {Attempt}/{MaxAttempts}",
                i + 1, maxAttempts);
            await Task.Delay(delayMs, cancellationToken);
        }

        _logger.LogWarning("Transaction receipt not received after {MaxAttempts} attempts. TxHash: {TxHash}",
            maxAttempts, txHash);
        return null;
    }
}
