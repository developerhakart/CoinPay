namespace CoinPay.Api.Models;

/// <summary>
/// Result of token balance validation
/// </summary>
public class TokenBalanceValidationResult
{
    /// <summary>
    /// Token address being validated
    /// </summary>
    public string TokenAddress { get; set; } = string.Empty;

    /// <summary>
    /// Current balance in wallet
    /// </summary>
    public decimal CurrentBalance { get; set; }

    /// <summary>
    /// Required amount for swap
    /// </summary>
    public decimal RequiredAmount { get; set; }

    /// <summary>
    /// Platform fee amount
    /// </summary>
    public decimal PlatformFee { get; set; }

    /// <summary>
    /// Total required (amount + fee)
    /// </summary>
    public decimal TotalRequired { get; set; }

    /// <summary>
    /// Whether wallet has sufficient balance
    /// </summary>
    public bool HasSufficientBalance { get; set; }

    /// <summary>
    /// Shortfall amount if insufficient
    /// </summary>
    public decimal ShortfallAmount { get; set; }
}
