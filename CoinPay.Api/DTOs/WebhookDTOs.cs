using System.ComponentModel.DataAnnotations;

namespace CoinPay.Api.DTOs;

/// <summary>
/// Request DTO for registering a webhook
/// </summary>
public class RegisterWebhookRequest
{
    /// <summary>
    /// Webhook URL to receive notifications
    /// </summary>
    [Required]
    [Url]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// List of events to subscribe to
    /// </summary>
    public List<string> Events { get; set; } = new() { "transaction.confirmed", "transaction.failed" };
}

/// <summary>
/// Response DTO for webhook registration
/// </summary>
public class WebhookRegistrationResponse
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public List<string> Events { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request DTO for updating a webhook
/// </summary>
public class UpdateWebhookRequest
{
    /// <summary>
    /// Webhook URL
    /// </summary>
    [Url]
    public string? Url { get; set; }

    /// <summary>
    /// List of events to subscribe to
    /// </summary>
    public List<string>? Events { get; set; }

    /// <summary>
    /// Whether the webhook is active
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Webhook payload sent to registered webhook URLs
/// </summary>
public class WebhookPayload
{
    /// <summary>
    /// Event name (e.g., "transaction.confirmed", "transaction.failed")
    /// </summary>
    public string Event { get; set; } = string.Empty;

    /// <summary>
    /// Event timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Transaction data
    /// </summary>
    public WebhookTransactionData Transaction { get; set; } = new();

    /// <summary>
    /// HMAC-SHA256 signature for verification
    /// </summary>
    public string Signature { get; set; } = string.Empty;
}

/// <summary>
/// Transaction data included in webhook payload
/// </summary>
public class WebhookTransactionData
{
    public int Id { get; set; }
    public string UserOpHash { get; set; } = string.Empty;
    public string? TransactionHash { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TokenAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

/// <summary>
/// Response DTO for webhook delivery log
/// </summary>
public class WebhookDeliveryLogResponse
{
    public int Id { get; set; }
    public int WebhookId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int TransactionId { get; set; }
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; }
    public int AttemptNumber { get; set; }
}
