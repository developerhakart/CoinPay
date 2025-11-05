using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Service for validating token balances before swap
/// </summary>
public interface ITokenBalanceValidationService
{
    /// <summary>
    /// Validates that a wallet has sufficient balance for a swap
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="walletAddress">Wallet address to check</param>
    /// <param name="tokenAddress">Token address to check balance for</param>
    /// <param name="requiredAmount">Required amount for swap</param>
    /// <returns>Validation result with balance details</returns>
    Task<TokenBalanceValidationResult> ValidateBalanceAsync(
        Guid userId,
        string walletAddress,
        string tokenAddress,
        decimal requiredAmount);
}
