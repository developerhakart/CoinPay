using System.ComponentModel.DataAnnotations;

namespace CoinPay.Api.DTOs;

/// <summary>
/// Request DTO for initiating a payout
/// </summary>
public class InitiatePayoutRequest
{
    [Required(ErrorMessage = "Bank account ID is required")]
    public Guid BankAccountId { get; set; }

    [Required(ErrorMessage = "USDC amount is required")]
    [Range(1.0, 1000000.0, ErrorMessage = "Amount must be between 1 and 1,000,000 USDC")]
    public decimal UsdcAmount { get; set; }
}

/// <summary>
/// Response DTO for payout initiation
/// </summary>
public class PayoutResponse
{
    public Guid Id { get; set; }
    public Guid BankAccountId { get; set; }
    public string? GatewayTransactionId { get; set; }
    public decimal UsdcAmount { get; set; }
    public decimal UsdAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal ConversionFee { get; set; }
    public decimal PayoutFee { get; set; }
    public decimal TotalFees { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public string? FailureReason { get; set; }

    // Bank account summary
    public BankAccountSummary? BankAccount { get; set; }
}

/// <summary>
/// Bank account summary for payout response
/// </summary>
public class BankAccountSummary
{
    public Guid Id { get; set; }
    public string AccountHolderName { get; set; } = string.Empty;
    public string LastFourDigits { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string? BankName { get; set; }
}

/// <summary>
/// Payout history list response
/// </summary>
public class PayoutHistoryResponse
{
    public List<PayoutResponse> Payouts { get; set; } = new();
    public int Total { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}

/// <summary>
/// Payout status response with detailed tracking
/// </summary>
public class PayoutStatusResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty; // initiated, converting, transferring, completed
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public string? FailureReason { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<PayoutStatusEvent> Events { get; set; } = new();
}

/// <summary>
/// Payout status event
/// </summary>
public class PayoutStatusEvent
{
    public string Event { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Conversion preview request
/// </summary>
public class ConversionPreviewRequest
{
    [Required]
    [Range(0.01, 1000000.0, ErrorMessage = "Amount must be between 0.01 and 1,000,000 USDC")]
    public decimal UsdcAmount { get; set; }
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
/// Detailed payout information response
/// </summary>
public class PayoutDetailsResponse
{
    public Guid Id { get; set; }
    public Guid BankAccountId { get; set; }
    public string? GatewayTransactionId { get; set; }
    public decimal UsdcAmount { get; set; }
    public decimal UsdAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal ConversionFee { get; set; }
    public decimal PayoutFee { get; set; }
    public decimal TotalFees { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public string? FailureReason { get; set; }
    public BankAccountSummary? BankAccount { get; set; }
}
