namespace CoinPay.Api.Models;

/// <summary>
/// Result of swap execution
/// </summary>
public class SwapExecutionResult
{
    /// <summary>
    /// Swap transaction ID
    /// </summary>
    public Guid SwapId { get; set; }

    /// <summary>
    /// Blockchain transaction hash
    /// </summary>
    public string? TransactionHash { get; set; }

    /// <summary>
    /// Swap status
    /// </summary>
    public SwapStatus Status { get; set; }

    /// <summary>
    /// Expected amount to receive
    /// </summary>
    public decimal ExpectedToAmount { get; set; }

    /// <summary>
    /// Minimum amount to receive after slippage
    /// </summary>
    public decimal MinimumReceived { get; set; }

    /// <summary>
    /// Platform fee amount
    /// </summary>
    public decimal PlatformFee { get; set; }

    /// <summary>
    /// Message describing the result
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
