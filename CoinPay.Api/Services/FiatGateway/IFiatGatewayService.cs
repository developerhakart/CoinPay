namespace CoinPay.Api.Services.FiatGateway;

/// <summary>
/// Interface for Fiat Gateway operations (RedotPay/Bridge integration)
/// Handles USDC to USD conversion and bank payouts
/// </summary>
public interface IFiatGatewayService
{
    /// <summary>
    /// Get current USDC to USD exchange rate
    /// </summary>
    /// <returns>Exchange rate information</returns>
    Task<ExchangeRateResponse> GetExchangeRateAsync();

    /// <summary>
    /// Calculate conversion preview
    /// </summary>
    /// <param name="usdcAmount">Amount in USDC</param>
    /// <returns>Conversion details including fees</returns>
    Task<ConversionPreviewResponse> GetConversionPreviewAsync(decimal usdcAmount);

    /// <summary>
    /// Initiate payout to bank account
    /// </summary>
    /// <param name="request">Payout request details</param>
    /// <returns>Payout initiation result</returns>
    Task<PayoutInitiationResponse> InitiatePayoutAsync(PayoutInitiationRequest request);

    /// <summary>
    /// Get payout status
    /// </summary>
    /// <param name="gatewayTransactionId">Gateway transaction ID</param>
    /// <returns>Current payout status</returns>
    Task<PayoutStatusResponse> GetPayoutStatusAsync(string gatewayTransactionId);

    /// <summary>
    /// Verify gateway connectivity and configuration
    /// </summary>
    /// <returns>True if gateway is available</returns>
    Task<bool> VerifyGatewayHealthAsync();
}

/// <summary>
/// Exchange rate response
/// </summary>
public class ExchangeRateResponse
{
    public decimal Rate { get; set; }
    public string BaseCurrency { get; set; } = "USDC";
    public string QuoteCurrency { get; set; } = "USD";
    public DateTime Timestamp { get; set; }
    public int ValidForSeconds { get; set; } = 30;
}

/// <summary>
/// Conversion preview response
/// </summary>
public class ConversionPreviewResponse
{
    public decimal UsdcAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal UsdAmountBeforeFees { get; set; }
    public decimal ConversionFeePercent { get; set; }
    public decimal ConversionFeeAmount { get; set; }
    public decimal PayoutFeeAmount { get; set; }
    public decimal TotalFees { get; set; }
    public decimal NetUsdAmount { get; set; }
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Payout initiation request
/// </summary>
public class PayoutInitiationRequest
{
    public int UserId { get; set; }
    public Guid BankAccountId { get; set; }
    public decimal UsdcAmount { get; set; }
    public string RoutingNumber { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string? BankName { get; set; }
}

/// <summary>
/// Payout initiation response
/// </summary>
public class PayoutInitiationResponse
{
    public bool Success { get; set; }
    public string GatewayTransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // pending, processing, completed, failed
    public decimal UsdcAmount { get; set; }
    public decimal UsdAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal TotalFees { get; set; }
    public decimal NetAmount { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Payout status response
/// </summary>
public class PayoutStatusResponse
{
    public string GatewayTransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // pending, processing, completed, failed, cancelled
    public DateTime? CompletedAt { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public string? FailureReason { get; set; }
    public PayoutStatusDetails? StatusDetails { get; set; }
}

/// <summary>
/// Detailed status information
/// </summary>
public class PayoutStatusDetails
{
    public string Stage { get; set; } = string.Empty; // initiated, converting, transferring, completed
    public DateTime LastUpdated { get; set; }
    public List<PayoutStatusEvent> Events { get; set; } = new();
}

/// <summary>
/// Status event in payout lifecycle
/// </summary>
public class PayoutStatusEvent
{
    public string Event { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }
}
