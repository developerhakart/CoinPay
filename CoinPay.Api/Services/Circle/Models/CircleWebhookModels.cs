using System.Text.Json.Serialization;

namespace CoinPay.Api.Services.Circle.Models;

/// <summary>
/// Circle webhook notification payload
/// </summary>
public class CircleWebhookNotification
{
    /// <summary>
    /// Webhook notification ID
    /// </summary>
    [JsonPropertyName("notificationId")]
    public string NotificationId { get; set; } = string.Empty;

    /// <summary>
    /// Webhook subscription ID
    /// </summary>
    [JsonPropertyName("subscriptionId")]
    public string SubscriptionId { get; set; } = string.Empty;

    /// <summary>
    /// Event type (e.g., "transactions.updated")
    /// </summary>
    [JsonPropertyName("notificationType")]
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>
    /// Notification timestamp
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Notification data containing transaction details
    /// </summary>
    [JsonPropertyName("notification")]
    public CircleWebhookData? Notification { get; set; }
}

/// <summary>
/// Webhook notification data
/// </summary>
public class CircleWebhookData
{
    /// <summary>
    /// Transaction ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Transaction state (INITIATED, PENDING_RISK_REVIEW, DENIED, QUEUED, SENT, CONFIRMED, COMPLETE, FAILED, CANCELLED)
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Blockchain network
    /// </summary>
    [JsonPropertyName("blockchain")]
    public string? Blockchain { get; set; }

    /// <summary>
    /// Transaction hash on blockchain
    /// </summary>
    [JsonPropertyName("txHash")]
    public string? TxHash { get; set; }

    /// <summary>
    /// Source wallet address
    /// </summary>
    [JsonPropertyName("sourceAddress")]
    public string? SourceAddress { get; set; }

    /// <summary>
    /// Destination wallet address
    /// </summary>
    [JsonPropertyName("destinationAddress")]
    public string? DestinationAddress { get; set; }

    /// <summary>
    /// Transaction amounts
    /// </summary>
    [JsonPropertyName("amounts")]
    public List<string>? Amounts { get; set; }

    /// <summary>
    /// Token ID (null for native currency)
    /// </summary>
    [JsonPropertyName("tokenId")]
    public string? TokenId { get; set; }

    /// <summary>
    /// Error code if transaction failed
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Error message if transaction failed
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Wallet ID
    /// </summary>
    [JsonPropertyName("walletId")]
    public string? WalletId { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    /// Transaction creation timestamp
    /// </summary>
    [JsonPropertyName("createDate")]
    public DateTime? CreateDate { get; set; }

    /// <summary>
    /// Transaction update timestamp
    /// </summary>
    [JsonPropertyName("updateDate")]
    public DateTime? UpdateDate { get; set; }
}
