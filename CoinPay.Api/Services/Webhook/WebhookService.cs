using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CoinPay.Api.DTOs;
using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using Polly;
using Polly.Retry;

namespace CoinPay.Api.Services.Webhook;

/// <summary>
/// Service for sending webhook notifications with retry logic
/// </summary>
public class WebhookService : IWebhookService
{
    private readonly IWebhookRepository _webhookRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookService> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public WebhookService(
        IWebhookRepository webhookRepository,
        IHttpClientFactory httpClientFactory,
        ILogger<WebhookService> logger)
    {
        _webhookRepository = webhookRepository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;

        // Configure retry policy: 3 attempts with exponential backoff
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Webhook delivery attempt {RetryCount} failed with status {StatusCode}. Retrying in {Delay}s",
                        retryCount,
                        outcome.Result?.StatusCode,
                        timespan.TotalSeconds);
                });
    }

    public async Task NotifyTransactionStatusChangeAsync(
        BlockchainTransaction transaction,
        TransactionStatus oldStatus,
        TransactionStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        // Determine event name
        var eventName = newStatus switch
        {
            TransactionStatus.Confirmed => "transaction.confirmed",
            TransactionStatus.Failed => "transaction.failed",
            _ => null
        };

        if (eventName == null)
        {
            _logger.LogDebug("No webhook event for status change from {OldStatus} to {NewStatus}",
                oldStatus, newStatus);
            return;
        }

        // Get user ID from wallet (assuming transaction has Wallet navigation property)
        if (transaction.Wallet == null)
        {
            _logger.LogWarning("Transaction {Id} has no associated wallet, cannot send webhooks",
                transaction.Id);
            return;
        }

        var userId = transaction.Wallet.UserId;

        // Get active webhooks for this user and event
        var webhooks = await _webhookRepository.GetActiveWebhooksForUserAsync(userId, cancellationToken);
        var relevantWebhooks = webhooks.Where(w => w.Events.Contains(eventName)).ToList();

        if (!relevantWebhooks.Any())
        {
            _logger.LogDebug("No active webhooks found for user {UserId} and event {Event}",
                userId, eventName);
            return;
        }

        _logger.LogInformation("Sending {Count} webhook(s) for transaction {Id} event {Event}",
            relevantWebhooks.Count, transaction.Id, eventName);

        // Send webhooks in parallel
        var tasks = relevantWebhooks.Select(webhook =>
            SendWebhookAsync(webhook, transaction, eventName, cancellationToken));

        await Task.WhenAll(tasks);
    }

    private async Task SendWebhookAsync(
        WebhookRegistration webhook,
        BlockchainTransaction transaction,
        string eventName,
        CancellationToken cancellationToken)
    {
        var payload = new WebhookPayload
        {
            Event = eventName,
            Timestamp = DateTime.UtcNow,
            Transaction = new WebhookTransactionData
            {
                Id = transaction.Id,
                UserOpHash = transaction.UserOpHash,
                TransactionHash = transaction.TransactionHash,
                FromAddress = transaction.FromAddress,
                ToAddress = transaction.ToAddress,
                Amount = transaction.AmountDecimal,
                TokenAddress = transaction.TokenAddress,
                Status = transaction.Status.ToString(),
                CreatedAt = transaction.CreatedAt,
                ConfirmedAt = transaction.ConfirmedAt
            },
            Signature = GenerateSignature(webhook.Secret, transaction.Id, transaction.UserOpHash, transaction.Status.ToString())
        };

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        int attemptNumber = 0;
        HttpResponseMessage? response = null;
        string? errorMessage = null;

        try
        {
            // Execute with retry policy
            response = await _retryPolicy.ExecuteAsync(async (context) =>
            {
                attemptNumber = context.Count;
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                _logger.LogDebug("Sending webhook to {Url} (attempt {Attempt})", webhook.Url, attemptNumber);

                return await client.PostAsync(webhook.Url, content, cancellationToken);
            }, new Context());

            // Log successful delivery
            await _webhookRepository.LogDeliveryAsync(new WebhookDeliveryLog
            {
                WebhookId = webhook.Id,
                EventName = eventName,
                TransactionId = transaction.Id,
                StatusCode = (int)response.StatusCode,
                Success = response.IsSuccessStatusCode,
                ResponseBody = await response.Content.ReadAsStringAsync(cancellationToken),
                AttemptNumber = attemptNumber
            }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Webhook delivered successfully to {Url} for transaction {Id} (attempt {Attempt})",
                    webhook.Url, transaction.Id, attemptNumber);
            }
            else
            {
                _logger.LogWarning(
                    "Webhook delivery failed after {Attempt} attempts to {Url} with status {StatusCode}",
                    attemptNumber, webhook.Url, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;

            _logger.LogError(ex,
                "Webhook delivery exception for {Url} after {Attempt} attempts",
                webhook.Url, attemptNumber);

            // Log failed delivery
            await _webhookRepository.LogDeliveryAsync(new WebhookDeliveryLog
            {
                WebhookId = webhook.Id,
                EventName = eventName,
                TransactionId = transaction.Id,
                StatusCode = response != null ? (int)response.StatusCode : 0,
                Success = false,
                ErrorMessage = errorMessage,
                AttemptNumber = attemptNumber
            }, cancellationToken);
        }
    }

    public string GenerateSignature(string secret, int transactionId, string userOpHash, string status)
    {
        var data = $"{transactionId}:{userOpHash}:{status}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }

    public bool VerifySignature(string signature, string secret, int transactionId, string userOpHash, string status)
    {
        var expectedSignature = GenerateSignature(secret, transactionId, userOpHash, status);
        return signature == expectedSignature;
    }
}
