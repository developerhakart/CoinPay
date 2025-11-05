using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoinPay.Api.DTOs.Exchange;
using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using CoinPay.Api.Services.Exchange.WhiteBit;
using CoinPay.Api.Services.Encryption;
using System.Security.Claims;

namespace CoinPay.Api.Controllers;

[ApiController]
[Route("api/exchange")]
[Authorize]
public class ExchangeController : ControllerBase
{
    private readonly IWhiteBitApiClient _whiteBitClient;
    private readonly IWhiteBitAuthService _authService;
    private readonly IExchangeConnectionRepository _connectionRepository;
    private readonly IExchangeCredentialEncryptionService _encryptionService;
    private readonly ILogger<ExchangeController> _logger;

    public ExchangeController(
        IWhiteBitApiClient whiteBitClient,
        IWhiteBitAuthService authService,
        IExchangeConnectionRepository connectionRepository,
        IExchangeCredentialEncryptionService encryptionService,
        ILogger<ExchangeController> logger)
    {
        _whiteBitClient = whiteBitClient;
        _authService = authService;
        _connectionRepository = connectionRepository;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    /// <summary>
    /// Connect WhiteBit account with API credentials
    /// </summary>
    [HttpPost("whitebit/connect")]
    [ProducesResponseType(typeof(ConnectWhiteBitResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> ConnectWhiteBit([FromBody] ConnectWhiteBitRequest request)
    {
        try
        {
            // Get user ID from authenticated user's JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userIdInt))
            {
                return Unauthorized(new { error = "Invalid user authentication" });
            }
            var userId = Guid.Parse($"00000000-0000-0000-0000-{userIdInt:D12}");

            // Check if already connected
            var existing = await _connectionRepository.GetByUserAndExchangeAsync(userId, "whitebit");
            if (existing != null)
            {
                return Conflict(new { error = "WhiteBit account already connected" });
            }

            // Validate credentials
            var isValid = await _authService.ValidateCredentialsAsync(request.ApiKey, request.ApiSecret);
            if (!isValid)
            {
                return BadRequest(new { error = "Invalid API credentials" });
            }

            // Encrypt credentials
            var encryptedApiKey = await _encryptionService.EncryptAsync(request.ApiKey, userId);
            var encryptedApiSecret = await _encryptionService.EncryptAsync(request.ApiSecret, userId);

            // Create connection
            var connection = new ExchangeConnection
            {
                UserId = userId,
                UserId1 = userIdInt, // Set the actual FK to Users.Id
                ExchangeName = "whitebit",
                ApiKeyEncrypted = encryptedApiKey,
                ApiSecretEncrypted = encryptedApiSecret,
                IsActive = true,
                LastValidatedAt = DateTime.UtcNow
            };

            connection = await _connectionRepository.CreateAsync(connection);

            _logger.LogInformation("WhiteBit connection created for user {UserId}", userId);

            return Ok(new ConnectWhiteBitResponse
            {
                ConnectionId = connection.Id,
                ExchangeName = "whitebit",
                Status = "active",
                ConnectedAt = connection.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect WhiteBit account");
            return StatusCode(500, new { error = "Failed to connect WhiteBit account" });
        }
    }

    /// <summary>
    /// Get WhiteBit connection status for current user
    /// </summary>
    [HttpGet("whitebit/status")]
    [ProducesResponseType(typeof(ExchangeConnectionStatusResponse), 200)]
    public async Task<IActionResult> GetWhiteBitStatus()
    {
        try
        {
            // Get user ID from authenticated user's JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userIdInt))
            {
                return Unauthorized(new { error = "Invalid user authentication" });
            }
            var userId = Guid.Parse($"00000000-0000-0000-0000-{userIdInt:D12}");

            var connection = await _connectionRepository.GetByUserAndExchangeAsync(userId, "whitebit");

            if (connection == null)
            {
                return Ok(new ExchangeConnectionStatusResponse
                {
                    Connected = false
                });
            }

            return Ok(new ExchangeConnectionStatusResponse
            {
                Connected = true,
                ConnectionId = connection.Id,
                ExchangeName = connection.ExchangeName,
                ConnectedAt = connection.CreatedAt,
                LastValidated = connection.LastValidatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get WhiteBit status");
            return StatusCode(500, new { error = "Failed to get connection status" });
        }
    }

    /// <summary>
    /// Get available WhiteBit Flex investment plans
    /// </summary>
    [HttpGet("whitebit/plans")]
    [ProducesResponseType(typeof(List<InvestmentPlanResponse>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetWhiteBitPlans()
    {
        try
        {
            // Get user ID from auth context (for MVP, using a test user ID)
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var connection = await _connectionRepository.GetByUserAndExchangeAsync(userId, "whitebit");
            if (connection == null)
            {
                return Unauthorized(new { error = "WhiteBit account not connected" });
            }

            // Decrypt credentials
            var apiKey = await _encryptionService.DecryptAsync(connection.ApiKeyEncrypted, userId);
            var apiSecret = await _encryptionService.DecryptAsync(connection.ApiSecretEncrypted, userId);

            // Get plans from WhiteBit
            var plansResponse = await _whiteBitClient.GetInvestmentPlansAsync(apiKey, apiSecret);

            var plans = plansResponse.Plans.Select(p => new InvestmentPlanResponse
            {
                PlanId = p.PlanId,
                Asset = p.Asset,
                Apy = p.Apy,
                ApyFormatted = $"{p.Apy:F2}%",
                MinAmount = p.MinAmount,
                MaxAmount = p.MaxAmount,
                Term = p.Term,
                Description = p.Description
            }).ToList();

            _logger.LogInformation("Retrieved {Count} investment plans for user {UserId}", plans.Count, userId);

            return Ok(plans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get investment plans");
            return StatusCode(500, new { error = "Failed to retrieve investment plans" });
        }
    }
}
