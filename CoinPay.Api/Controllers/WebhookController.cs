using System.Security.Cryptography;
using CoinPay.Api.DTOs;
using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CoinPay.Api.Controllers;

/// <summary>
/// Controller for webhook management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WebhookController : ControllerBase
{
    private readonly IWebhookRepository _webhookRepository;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(
        IWebhookRepository webhookRepository,
        ILogger<WebhookController> logger)
    {
        _webhookRepository = webhookRepository;
        _logger = logger;
    }

    /// <summary>
    /// Register a new webhook
    /// </summary>
    /// <param name="request">Webhook registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created webhook registration</returns>
    [HttpPost]
    [ProducesResponseType(typeof(WebhookRegistrationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WebhookRegistrationResponse>> RegisterWebhook(
        [FromBody] RegisterWebhookRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering webhook for URL: {Url}", request.Url);

        // Validate URL
        if (!Uri.IsWellFormedUriString(request.Url, UriKind.Absolute))
        {
            return BadRequest(new { error = "Invalid webhook URL format" });
        }

        // Validate events
        var validEvents = new[] { "transaction.confirmed", "transaction.failed" };
        if (request.Events.Any(e => !validEvents.Contains(e)))
        {
            return BadRequest(new { error = "Invalid event names. Valid events: transaction.confirmed, transaction.failed" });
        }

        // For now, use a hardcoded user (TODO: Replace with actual authentication)
        var userId = 1;

        // Generate webhook secret
        var secret = GenerateWebhookSecret();

        var webhook = new WebhookRegistration
        {
            UserId = userId,
            Url = request.Url,
            Secret = secret,
            Events = string.Join(",", request.Events),
            IsActive = true
        };

        await _webhookRepository.CreateAsync(webhook, cancellationToken);

        var response = MapToResponse(webhook);

        _logger.LogInformation("Webhook {Id} registered successfully", webhook.Id);

        return CreatedAtAction(nameof(GetWebhook), new { id = webhook.Id }, response);
    }

    /// <summary>
    /// Get webhook by ID
    /// </summary>
    /// <param name="id">Webhook ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Webhook registration</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WebhookRegistrationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WebhookRegistrationResponse>> GetWebhook(
        int id,
        CancellationToken cancellationToken)
    {
        var webhook = await _webhookRepository.GetByIdAsync(id, cancellationToken);

        if (webhook == null)
        {
            return NotFound(new { error = "Webhook not found" });
        }

        // TODO: Verify user owns this webhook
        return Ok(MapToResponse(webhook));
    }

    /// <summary>
    /// Get all webhooks for authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of webhook registrations</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<WebhookRegistrationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WebhookRegistrationResponse>>> GetAllWebhooks(
        CancellationToken cancellationToken)
    {
        // For now, use a hardcoded user (TODO: Replace with actual authentication)
        var userId = 1;

        var webhooks = await _webhookRepository.GetByUserIdAsync(userId, cancellationToken);

        var response = webhooks.Select(MapToResponse).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Update webhook
    /// </summary>
    /// <param name="id">Webhook ID</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated webhook registration</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(WebhookRegistrationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WebhookRegistrationResponse>> UpdateWebhook(
        int id,
        [FromBody] UpdateWebhookRequest request,
        CancellationToken cancellationToken)
    {
        var webhook = await _webhookRepository.GetByIdAsync(id, cancellationToken);

        if (webhook == null)
        {
            return NotFound(new { error = "Webhook not found" });
        }

        // TODO: Verify user owns this webhook

        // Update fields
        if (request.Url != null)
        {
            if (!Uri.IsWellFormedUriString(request.Url, UriKind.Absolute))
            {
                return BadRequest(new { error = "Invalid webhook URL format" });
            }
            webhook.Url = request.Url;
        }

        if (request.Events != null)
        {
            var validEvents = new[] { "transaction.confirmed", "transaction.failed" };
            if (request.Events.Any(e => !validEvents.Contains(e)))
            {
                return BadRequest(new { error = "Invalid event names" });
            }
            webhook.Events = string.Join(",", request.Events);
        }

        if (request.IsActive.HasValue)
        {
            webhook.IsActive = request.IsActive.Value;
        }

        await _webhookRepository.UpdateAsync(webhook, cancellationToken);

        _logger.LogInformation("Webhook {Id} updated successfully", id);

        return Ok(MapToResponse(webhook));
    }

    /// <summary>
    /// Delete webhook
    /// </summary>
    /// <param name="id">Webhook ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteWebhook(
        int id,
        CancellationToken cancellationToken)
    {
        var webhook = await _webhookRepository.GetByIdAsync(id, cancellationToken);

        if (webhook == null)
        {
            return NotFound(new { error = "Webhook not found" });
        }

        // TODO: Verify user owns this webhook

        await _webhookRepository.DeleteAsync(id, cancellationToken);

        _logger.LogInformation("Webhook {Id} deleted successfully", id);

        return NoContent();
    }

    /// <summary>
    /// Get delivery logs for a webhook
    /// </summary>
    /// <param name="id">Webhook ID</param>
    /// <param name="limit">Maximum number of logs to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of delivery logs</returns>
    [HttpGet("{id}/logs")]
    [ProducesResponseType(typeof(List<WebhookDeliveryLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<WebhookDeliveryLogResponse>>> GetDeliveryLogs(
        int id,
        [FromQuery] int limit = 100,
        CancellationToken cancellationToken = default)
    {
        var webhook = await _webhookRepository.GetByIdAsync(id, cancellationToken);

        if (webhook == null)
        {
            return NotFound(new { error = "Webhook not found" });
        }

        // TODO: Verify user owns this webhook

        var logs = await _webhookRepository.GetDeliveryLogsAsync(id, limit, cancellationToken);

        var response = logs.Select(l => new WebhookDeliveryLogResponse
        {
            Id = l.Id,
            WebhookId = l.WebhookId,
            EventName = l.EventName,
            TransactionId = l.TransactionId,
            StatusCode = l.StatusCode,
            Success = l.Success,
            ErrorMessage = l.ErrorMessage,
            Timestamp = l.Timestamp,
            AttemptNumber = l.AttemptNumber
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Generate a cryptographically secure webhook secret
    /// </summary>
    private static string GenerateWebhookSecret()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Map webhook registration to response DTO
    /// </summary>
    private static WebhookRegistrationResponse MapToResponse(WebhookRegistration webhook)
    {
        return new WebhookRegistrationResponse
        {
            Id = webhook.Id,
            Url = webhook.Url,
            Secret = webhook.Secret,
            Events = webhook.Events.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            IsActive = webhook.IsActive,
            CreatedAt = webhook.CreatedAt
        };
    }
}
