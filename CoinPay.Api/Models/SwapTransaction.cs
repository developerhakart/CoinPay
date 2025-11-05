namespace CoinPay.Api.Models;

/// <summary>
/// Represents a token swap transaction
/// </summary>
public class SwapTransaction
{
    /// <summary>
    /// Unique swap identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User who initiated the swap
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Wallet address performing the swap
    /// </summary>
    public string WalletAddress { get; set; } = string.Empty;

    /// <summary>
    /// Source token address
    /// </summary>
    public string FromToken { get; set; } = string.Empty;

    /// <summary>
    /// Destination token address
    /// </summary>
    public string ToToken { get; set; } = string.Empty;

    /// <summary>
    /// Source token symbol (USDC, WETH, WMATIC)
    /// </summary>
    public string FromTokenSymbol { get; set; } = string.Empty;

    /// <summary>
    /// Destination token symbol
    /// </summary>
    public string ToTokenSymbol { get; set; } = string.Empty;

    /// <summary>
    /// Amount of source token swapped
    /// </summary>
    public decimal FromAmount { get; set; }

    /// <summary>
    /// Amount of destination token received
    /// </summary>
    public decimal ToAmount { get; set; }

    /// <summary>
    /// Exchange rate at time of swap
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Platform fee amount in source token
    /// </summary>
    public decimal PlatformFee { get; set; }

    /// <summary>
    /// Platform fee percentage (e.g., 0.5 for 0.5%)
    /// </summary>
    public decimal PlatformFeePercentage { get; set; }

    /// <summary>
    /// Actual gas used for transaction
    /// </summary>
    public string? GasUsed { get; set; }

    /// <summary>
    /// Actual gas cost in native token
    /// </summary>
    public decimal? GasCost { get; set; }

    /// <summary>
    /// Slippage tolerance percentage used
    /// </summary>
    public decimal SlippageTolerance { get; set; }

    /// <summary>
    /// Price impact percentage
    /// </summary>
    public decimal? PriceImpact { get; set; }

    /// <summary>
    /// Minimum amount configured to receive
    /// </summary>
    public decimal MinimumReceived { get; set; }

    /// <summary>
    /// DEX provider used (1inch, 0x)
    /// </summary>
    public string DexProvider { get; set; } = string.Empty;

    /// <summary>
    /// Blockchain transaction hash
    /// </summary>
    public string? TransactionHash { get; set; }

    /// <summary>
    /// Swap status
    /// </summary>
    public SwapStatus Status { get; set; }

    /// <summary>
    /// Error message if swap failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Swap creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Swap confirmation timestamp
    /// </summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Swap transaction status
/// </summary>
public enum SwapStatus
{
    /// <summary>
    /// Swap initiated but not yet confirmed on blockchain
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Swap confirmed on blockchain
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Swap failed or reverted
    /// </summary>
    Failed = 2
}
