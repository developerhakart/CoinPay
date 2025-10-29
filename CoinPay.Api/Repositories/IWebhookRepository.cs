using CoinPay.Api.Models;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository interface for webhook operations
/// </summary>
public interface IWebhookRepository
{
    /// <summary>
    /// Create a new webhook registration
    /// </summary>
    Task<WebhookRegistration> CreateAsync(WebhookRegistration webhook, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get webhook by ID
    /// </summary>
    Task<WebhookRegistration?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all webhooks for a user
    /// </summary>
    Task<List<WebhookRegistration>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active webhooks for a user
    /// </summary>
    Task<List<WebhookRegistration>> GetActiveWebhooksForUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a webhook registration
    /// </summary>
    Task<WebhookRegistration> UpdateAsync(WebhookRegistration webhook, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a webhook registration
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Log a webhook delivery attempt
    /// </summary>
    Task<WebhookDeliveryLog> LogDeliveryAsync(WebhookDeliveryLog log, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get delivery logs for a webhook
    /// </summary>
    Task<List<WebhookDeliveryLog>> GetDeliveryLogsAsync(int webhookId, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get delivery logs for a transaction
    /// </summary>
    Task<List<WebhookDeliveryLog>> GetDeliveryLogsByTransactionAsync(int transactionId, CancellationToken cancellationToken = default);
}
