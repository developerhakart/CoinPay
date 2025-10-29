namespace CoinPay.Api.Models;

/// <summary>
/// Represents a registered webhook for receiving transaction status notifications
/// </summary>
public class WebhookRegistration
{
    /// <summary>
    /// Unique webhook ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User ID (owner of the webhook)
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Webhook URL to receive notifications
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Secret key for HMAC signature verification
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated list of events to subscribe to
    /// (e.g., "transaction.confirmed,transaction.failed")
    /// </summary>
    public string Events { get; set; } = string.Empty;

    /// <summary>
    /// Whether the webhook is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Webhook creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property to delivery logs
    /// </summary>
    public List<WebhookDeliveryLog> DeliveryLogs { get; set; } = new();
}

/// <summary>
/// Represents a webhook delivery attempt log
/// </summary>
public class WebhookDeliveryLog
{
    /// <summary>
    /// Log entry ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Webhook registration ID
    /// </summary>
    public int WebhookId { get; set; }

    /// <summary>
    /// Event name (e.g., "transaction.confirmed")
    /// </summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// Transaction ID that triggered the webhook
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// HTTP status code received
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Whether delivery was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response body from webhook endpoint
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Delivery attempt timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Retry attempt number (0 for first attempt)
    /// </summary>
    public int AttemptNumber { get; set; }

    /// <summary>
    /// Navigation property to webhook registration
    /// </summary>
    public WebhookRegistration? Webhook { get; set; }

    /// <summary>
    /// Navigation property to transaction
    /// </summary>
    public BlockchainTransaction? Transaction { get; set; }
}
