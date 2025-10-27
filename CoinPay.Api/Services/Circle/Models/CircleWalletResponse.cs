namespace CoinPay.Api.Services.Circle.Models;

/// <summary>
/// Response containing Circle wallet details.
/// </summary>
public class CircleWalletResponse
{
    /// <summary>
    /// The unique wallet identifier.
    /// </summary>
    public string WalletId { get; set; } = string.Empty;

    /// <summary>
    /// The blockchain address of the wallet.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// The blockchain network (e.g., "MATIC-AMOY" for Polygon testnet).
    /// </summary>
    public string Blockchain { get; set; } = string.Empty;

    /// <summary>
    /// The wallet type (e.g., "SCA" for Smart Contract Account).
    /// </summary>
    public string WalletType { get; set; } = string.Empty;

    /// <summary>
    /// The Circle user ID that owns this wallet.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the wallet was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The current balance of the wallet (if available).
    /// </summary>
    public decimal? Balance { get; set; }

    /// <summary>
    /// The currency of the balance (e.g., "USDC").
    /// </summary>
    public string? BalanceCurrency { get; set; }
}
