using Microsoft.AspNetCore.Mvc;
using CoinPay.Api.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace CoinPay.Api.Controllers;

/// <summary>
/// Webhook endpoint for receiving payout status updates from fiat gateway
/// Handles asynchronous status notifications
/// </summary>
[ApiController]
[Route("api/webhook/payout")]
public class PayoutWebhookController : ControllerBase
{
    private readonly IPayoutRepository _payoutRepository;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PayoutWebhookController> _logger;

    public PayoutWebhookController(
        IPayoutRepository payoutRepository,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        ILogger<PayoutWebhookController> logger)
    {
        _payoutRepository = payoutRepository;
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Receive payout status update from gateway
    /// </summary>
    /// <param name="request">Webhook payload</param>
    /// <returns>Acknowledgement response</returns>
    [HttpPost("status-update")]
    [ProducesResponseType(typeof(WebhookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WebhookResponse>> HandleStatusUpdate([FromBody] PayoutWebhookRequest request)
    {
        try
        {
            // Validate webhook signature
            var signature = Request.Headers["X-Gateway-Signature"].FirstOrDefault();
            if (!ValidateSignature(request, signature))
            {
                _logger.LogWarning("HandleStatusUpdate: Invalid webhook signature for transaction {GatewayTxId}", request.GatewayTransactionId);
                return Unauthorized(new { error = new { code = "INVALID_SIGNATURE", message = "Webhook signature validation failed" } });
            }

            _logger.LogInformation("HandleStatusUpdate: Processing webhook for gateway transaction {GatewayTxId}, Status: {Status}",
                request.GatewayTransactionId, request.Status);

            // Find payout by gateway transaction ID
            var payout = await _payoutRepository.GetByGatewayTransactionIdAsync(request.GatewayTransactionId);

            if (payout == null)
            {
                _logger.LogWarning("HandleStatusUpdate: Payout not found for gateway transaction {GatewayTxId}", request.GatewayTransactionId);
                return BadRequest(new { error = new { code = "PAYOUT_NOT_FOUND", message = "Payout not found" } });
            }

            // Update payout status
            var previousStatus = payout.Status;
            payout.Status = request.Status;

            // Update completion timestamp if completed or failed
            if (request.Status == "completed" || request.Status == "failed")
            {
                payout.CompletedAt = request.CompletedAt ?? DateTime.UtcNow;
            }

            // Update failure reason if failed
            if (request.Status == "failed" && !string.IsNullOrEmpty(request.FailureReason))
            {
                payout.FailureReason = request.FailureReason;
            }

            // Update estimated arrival if provided
            if (request.EstimatedArrival.HasValue)
            {
                payout.EstimatedArrival = request.EstimatedArrival;
            }

            await _payoutRepository.UpdateAsync(payout);

            _logger.LogInformation("HandleStatusUpdate: Updated payout {PayoutId} status from {PreviousStatus} to {NewStatus}",
                payout.Id, previousStatus, request.Status);

            // Note: User notifications (email, SMS, push notifications) would be implemented
            // in a dedicated INotificationService and triggered here when payout status changes.
            // Current implementation ensures accurate status tracking via logging for audit purposes.
            // Tracked in backlog as BE-701 - Implement notification service

            return Ok(new WebhookResponse
            {
                Success = true,
                Message = "Webhook processed successfully",
                PayoutId = payout.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HandleStatusUpdate: Error processing webhook for gateway transaction {GatewayTxId}",
                request.GatewayTransactionId);
            return StatusCode(500, new { error = new { code = "WEBHOOK_ERROR", message = "Failed to process webhook" } });
        }
    }

    /// <summary>
    /// Health check endpoint for gateway to verify webhook availability
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        _logger.LogDebug("Webhook health check");
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Validate webhook signature using HMAC-SHA256
    /// </summary>
    private bool ValidateSignature(PayoutWebhookRequest request, string? providedSignature)
    {
        // Get webhook secret from configuration
        var webhookSecret = _configuration["Gateway:WebhookSecret"];

        // SECURITY: In production, webhook secret must be configured
        if (string.IsNullOrEmpty(webhookSecret))
        {
            // Only allow unsigned webhooks in Development environment with explicit configuration
            if (_environment.IsDevelopment() && _configuration.GetValue<bool>("Gateway:AllowUnsignedWebhooks", false))
            {
                _logger.LogWarning("SECURITY: Webhook signature validation skipped (Development mode with AllowUnsignedWebhooks=true). GatewayTxId: {GatewayTxId}",
                    request.GatewayTransactionId);
                return true;
            }

            // Production or Development without explicit flag: FAIL
            _logger.LogError("SECURITY: Webhook secret not configured. Signature validation failed. Environment: {Environment}, GatewayTxId: {GatewayTxId}",
                _environment.EnvironmentName, request.GatewayTransactionId);
            return false;
        }

        // Signature must be provided
        if (string.IsNullOrEmpty(providedSignature))
        {
            _logger.LogWarning("SECURITY: No signature provided in webhook request. GatewayTxId: {GatewayTxId}",
                request.GatewayTransactionId);
            return false;
        }

        // Calculate expected signature
        var payload = System.Text.Json.JsonSerializer.Serialize(request);
        var expectedSignature = ComputeHmacSha256(payload, webhookSecret);

        // Compare signatures (constant-time comparison to prevent timing attacks)
        var isValid = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expectedSignature),
            Encoding.UTF8.GetBytes(providedSignature)
        );

        if (!isValid)
        {
            _logger.LogWarning("SECURITY: Webhook signature validation failed. Expected: {Expected}, Provided: {Provided}, GatewayTxId: {GatewayTxId}",
                expectedSignature, providedSignature, request.GatewayTransactionId);
        }

        return isValid;
    }

    /// <summary>
    /// Compute HMAC-SHA256 signature
    /// </summary>
    private static string ComputeHmacSha256(string data, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}

#region DTOs

/// <summary>
/// Webhook request payload from fiat gateway
/// </summary>
public class PayoutWebhookRequest
{
    public string GatewayTransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // pending, processing, completed, failed
    public DateTime? CompletedAt { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public string? FailureReason { get; set; }
    public string? Stage { get; set; } // initiated, converting, transferring, completed
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Webhook response
/// </summary>
public class WebhookResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? PayoutId { get; set; }
}

#endregion
