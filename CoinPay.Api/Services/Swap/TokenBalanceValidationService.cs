using CoinPay.Api.Models;
using CoinPay.Api.Services.Wallet;
using Microsoft.Extensions.Logging;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Service to validate user has sufficient token balance for swaps
/// </summary>
public class TokenBalanceValidationService : ITokenBalanceValidationService
{
    private readonly IWalletService _walletService;
    private readonly IFeeCalculationService _feeService;
    private readonly ILogger<TokenBalanceValidationService> _logger;

    public TokenBalanceValidationService(
        IWalletService walletService,
        IFeeCalculationService feeService,
        ILogger<TokenBalanceValidationService> logger)
    {
        _walletService = walletService;
        _feeService = feeService;
        _logger = logger;
    }

    public async Task<TokenBalanceValidationResult> ValidateBalanceAsync(
        Guid userId,
        string walletAddress,
        string tokenAddress,
        decimal requiredAmount)
    {
        _logger.LogInformation(
            "Validating balance for user {UserId}: Wallet={Wallet}, Token={Token}, Required={Required}",
            userId,
            walletAddress,
            tokenAddress,
            requiredAmount);

        // Get wallet balance
        var balanceResult = await _walletService.GetWalletBalanceAsync(walletAddress, forceRefresh: true);

        // For MVP, we'll use a simplified balance check
        // In production, this should query actual token balances from Circle or blockchain
        var currentBalance = GetTokenBalance(balanceResult, tokenAddress);

        // Calculate platform fee
        var platformFee = await _feeService.CalculateSwapFeeAsync(tokenAddress, requiredAmount);

        // Total required = amount + fee
        var totalRequired = requiredAmount + platformFee;

        // Check if sufficient
        var hasSufficient = currentBalance >= totalRequired;
        var shortfall = hasSufficient ? 0 : (totalRequired - currentBalance);

        var result = new TokenBalanceValidationResult
        {
            TokenAddress = tokenAddress,
            CurrentBalance = currentBalance,
            RequiredAmount = requiredAmount,
            PlatformFee = platformFee,
            TotalRequired = totalRequired,
            HasSufficientBalance = hasSufficient,
            ShortfallAmount = shortfall
        };

        if (!hasSufficient)
        {
            _logger.LogWarning(
                "Insufficient balance for user {UserId}: Required={Required}, Available={Available}, Shortfall={Shortfall}",
                userId,
                totalRequired,
                currentBalance,
                shortfall);
        }
        else
        {
            _logger.LogInformation(
                "Balance validation passed for user {UserId}: Available={Available}, Required={Required}",
                userId,
                currentBalance,
                totalRequired);
        }

        return result;
    }

    private decimal GetTokenBalance(object balanceResult, string tokenAddress)
    {
        // For MVP, return mock balances
        // In production, parse the actual balance from Circle API response
        // or query blockchain directly

        // Mock balances for testing
        return 1000m; // Assume user has 1000 units of any token
    }
}
