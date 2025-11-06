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
    /// Extract and validate user ID from JWT token claims
    /// </summary>
    /// <returns>User ID as Guid, or null if invalid/missing</returns>
    private Guid? GetAuthenticatedUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userIdInt))
        {
            _logger.LogWarning("GetAuthenticatedUserId: Invalid user authentication - unable to parse user ID from token");
            return null;
        }

        // Ensure userIdInt is positive
        if (userIdInt <= 0)
        {
            _logger.LogError("GetAuthenticatedUserId: User ID must be positive - {UserId}", userIdInt);
            return null;
        }

        return Guid.Parse($"00000000-0000-0000-0000-{userIdInt:D12}");
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
        // Validate request object exists
        if (request == null)
        {
            _logger.LogWarning("ConnectWhiteBit: Null request body");
            return BadRequest(new { error = "Request body is required" });
        }

        // Validate API credentials format
        if (string.IsNullOrWhiteSpace(request.ApiKey))
        {
            _logger.LogWarning("ConnectWhiteBit: Missing API key");
            return BadRequest(new { error = "API key is required" });
        }

        if (string.IsNullOrWhiteSpace(request.ApiSecret))
        {
            _logger.LogWarning("ConnectWhiteBit: Missing API secret");
            return BadRequest(new { error = "API secret is required" });
        }

        // Validate credential length (adjust limits based on WhiteBit requirements)
        const int minCredentialLength = 10;
        const int maxCredentialLength = 256;

        if (request.ApiKey.Length < minCredentialLength || request.ApiKey.Length > maxCredentialLength)
        {
            _logger.LogWarning("ConnectWhiteBit: API key length invalid - Length: {Length}", request.ApiKey.Length);
            return BadRequest(new { error = "API key format is invalid" });
        }

        if (request.ApiSecret.Length < minCredentialLength || request.ApiSecret.Length > maxCredentialLength)
        {
            _logger.LogWarning("ConnectWhiteBit: API secret length invalid - Length: {Length}", request.ApiSecret.Length);
            return BadRequest(new { error = "API secret format is invalid" });
        }

        try
        {
            // Get user ID from authenticated user's JWT token
            var userId = GetAuthenticatedUserId();
            if (userId == null)
            {
                _logger.LogWarning("ConnectWhiteBit: Failed to extract authenticated user ID");
                return Unauthorized(new { error = "Invalid user authentication" });
            }

            // Get the integer userId for FK relationship
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userIdInt = int.Parse(userIdClaim!);

            // Check if already connected
            var existing = await _connectionRepository.GetByUserAndExchangeAsync(userId.Value, "whitebit");
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
            var encryptedApiKey = await _encryptionService.EncryptAsync(request.ApiKey, userId.Value);
            var encryptedApiSecret = await _encryptionService.EncryptAsync(request.ApiSecret, userId.Value);

            // Create connection
            var connection = new ExchangeConnection
            {
                UserId = userId.Value,
                UserId1 = userIdInt, // Set the actual FK to Users.Id
                ExchangeName = "whitebit",
                ApiKeyEncrypted = encryptedApiKey,
                ApiSecretEncrypted = encryptedApiSecret,
                IsActive = true,
                LastValidatedAt = DateTime.UtcNow
            };

            connection = await _connectionRepository.CreateAsync(connection);

            _logger.LogInformation("WhiteBit connection created for user {UserId}", userId.Value);

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
            var userId = GetAuthenticatedUserId();
            if (userId == null)
            {
                _logger.LogWarning("GetWhiteBitStatus: Failed to extract authenticated user ID");
                return Unauthorized(new { error = "Invalid user authentication" });
            }

            var connection = await _connectionRepository.GetByUserAndExchangeAsync(userId.Value, "whitebit");

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
            // Get user ID from authenticated user's JWT token
            var userId = GetAuthenticatedUserId();
            if (userId == null)
            {
                _logger.LogWarning("GetWhiteBitPlans: Failed to extract authenticated user ID");
                return Unauthorized(new { error = "Invalid user authentication" });
            }

            var connection = await _connectionRepository.GetByUserAndExchangeAsync(userId.Value, "whitebit");
            if (connection == null)
            {
                _logger.LogWarning("GetWhiteBitPlans: WhiteBit account not connected for user {UserId}", userId.Value);
                return Unauthorized(new { error = "WhiteBit account not connected" });
            }

            // Decrypt credentials
            var apiKey = await _encryptionService.DecryptAsync(connection.ApiKeyEncrypted, userId.Value);
            var apiSecret = await _encryptionService.DecryptAsync(connection.ApiSecretEncrypted, userId.Value);

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

            _logger.LogInformation("Retrieved {Count} investment plans for user {UserId}", plans.Count, userId.Value);

            return Ok(plans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get investment plans");
            return StatusCode(500, new { error = "Failed to retrieve investment plans" });
        }
    }
}
