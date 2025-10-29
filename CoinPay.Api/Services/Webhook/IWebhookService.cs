using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Webhook;

/// <summary>
/// Service for sending webhook notifications
/// </summary>
public interface IWebhookService
{
    /// <summary>
    /// Send webhook notification for transaction status change
    /// </summary>
    Task NotifyTransactionStatusChangeAsync(
        BlockchainTransaction transaction,
        TransactionStatus oldStatus,
        TransactionStatus newStatus,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate HMAC signature for webhook payload
    /// </summary>
    string GenerateSignature(string secret, int transactionId, string userOpHash, string status);

    /// <summary>
    /// Verify webhook signature
    /// </summary>
    bool VerifySignature(string signature, string secret, int transactionId, string userOpHash, string status);
}
