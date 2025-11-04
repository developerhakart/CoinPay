using CoinPay.Api.Data;
using CoinPay.Api.Services.Circle.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Services.Circle;

/// <summary>
/// Handles Circle webhook notifications for transaction updates
/// </summary>
public interface ICircleWebhookHandler
{
    /// <summary>
    /// Process a Circle webhook notification
    /// </summary>
    Task ProcessWebhookAsync(CircleWebhookNotification notification, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of Circle webhook handler
/// </summary>
public class CircleWebhookHandler : ICircleWebhookHandler
{
    private readonly ILogger<CircleWebhookHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CircleWebhookHandler(
        ILogger<CircleWebhookHandler> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public async Task ProcessWebhookAsync(CircleWebhookNotification notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing Circle webhook: Type={Type}, NotificationId={NotificationId}, TransactionId={TransactionId}",
            notification.NotificationType,
            notification.NotificationId,
            notification.Notification?.Id);

        // Only process transaction updates
        if (notification.NotificationType != "transactions.updated")
        {
            _logger.LogDebug("Ignoring webhook notification type: {Type}", notification.NotificationType);
            return;
        }

        if (notification.Notification == null)
        {
            _logger.LogWarning("Webhook notification data is null");
            return;
        }

        var transactionId = notification.Notification.Id;
        var state = notification.Notification.State;

        if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(state))
        {
            _logger.LogWarning("Webhook notification missing transaction ID or state");
            return;
        }

        // Update transaction in database
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var transaction = await db.Transactions
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId, cancellationToken);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction not found for CircleTransactionId: {TransactionId}", transactionId);
            return;
        }

        var previousStatus = transaction.Status;

        // Map Circle state to our status
        transaction.Status = state.ToUpper() switch
        {
            "CONFIRMED" => "Completed",
            "COMPLETE" => "Completed",
            "FAILED" => "Failed",
            "CANCELLED" => "Failed",
            "DENIED" => "Failed",
            _ => "Pending"
        };

        // Set completion timestamp if status changed
        if (transaction.Status != "Pending" && previousStatus == "Pending")
        {
            transaction.CompletedAt = DateTime.UtcNow;

            _logger.LogInformation(
                "Transaction {Id} status updated via webhook: {OldStatus} → {NewStatus}, CircleTransactionId: {CircleTransactionId}, TxHash: {TxHash}",
                transaction.Id,
                previousStatus,
                transaction.Status,
                transactionId,
                notification.Notification.TxHash ?? "N/A");
        }
        else if (transaction.Status == previousStatus)
        {
            _logger.LogDebug("Transaction {Id} status unchanged: {Status}", transaction.Id, transaction.Status);
            return; // No changes, skip save
        }

        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Transaction {Id} updated successfully via webhook",
            transaction.Id);
    }
}
