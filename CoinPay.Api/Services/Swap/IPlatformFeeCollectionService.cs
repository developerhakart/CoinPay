namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Service for collecting platform fees from swaps
/// </summary>
public interface IPlatformFeeCollectionService
{
    /// <summary>
    /// Collects platform fee for a completed swap
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="swapId">Swap transaction ID</param>
    /// <param name="feeAmount">Fee amount to collect</param>
    Task CollectSwapFeeAsync(Guid userId, Guid swapId, decimal feeAmount);
}
