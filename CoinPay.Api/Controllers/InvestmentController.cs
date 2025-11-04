using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoinPay.Api.DTOs.Exchange;
using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using CoinPay.Api.Services.Investment;
using CoinPay.Api.Services.Exchange.WhiteBit;
using CoinPay.Api.Services.Encryption;
using System.Security.Claims;

namespace CoinPay.Api.Controllers;

[ApiController]
[Route("api/investment")]
[Authorize]
public class InvestmentController : ControllerBase
{
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IExchangeConnectionRepository _connectionRepository;
    private readonly IWhiteBitApiClient _whiteBitClient;
    private readonly IRewardCalculationService _rewardCalculation;
    private readonly IExchangeCredentialEncryptionService _encryptionService;
    private readonly ILogger<InvestmentController> _logger;

    public InvestmentController(
        IInvestmentRepository investmentRepository,
        IExchangeConnectionRepository connectionRepository,
        IWhiteBitApiClient whiteBitClient,
        IRewardCalculationService rewardCalculation,
        IExchangeCredentialEncryptionService encryptionService,
        ILogger<InvestmentController> logger)
    {
        _investmentRepository = investmentRepository;
        _connectionRepository = connectionRepository;
        _whiteBitClient = whiteBitClient;
        _rewardCalculation = rewardCalculation;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    /// <summary>
    /// Create new investment position
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateInvestmentResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateInvestment([FromBody] CreateInvestmentRequest request)
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

            // Get exchange connection
            var connection = await _connectionRepository.GetByUserAndExchangeAsync(userId, "whitebit");
            if (connection == null)
            {
                return BadRequest(new { error = "WhiteBit account not connected" });
            }

            // Decrypt credentials
            var apiKey = await _encryptionService.DecryptAsync(connection.ApiKeyEncrypted, userId);
            var apiSecret = await _encryptionService.DecryptAsync(connection.ApiSecretEncrypted, userId);

            // Create investment via WhiteBit API
            var whiteBitResponse = await _whiteBitClient.CreateInvestmentAsync(
                apiKey, apiSecret, request.PlanId, request.Amount);

            // Get APY from plan (hardcoded for MVP)
            decimal apy = 8.50m;

            // Create investment position in database
            var position = new InvestmentPosition
            {
                UserId = userId,
                UserId1 = userIdInt, // Set the actual FK to Users.Id
                ExchangeConnectionId = connection.Id,
                ExchangeName = "whitebit",
                ExternalPositionId = whiteBitResponse.InvestmentId,
                PlanId = request.PlanId,
                Asset = "USDC",
                PrincipalAmount = request.Amount,
                CurrentValue = request.Amount,
                AccruedRewards = 0,
                Apy = apy,
                Status = InvestmentStatus.Active,
                StartDate = DateTime.UtcNow
            };

            position = await _investmentRepository.CreateAsync(position);

            // Create transaction record
            var transaction = new InvestmentTransaction
            {
                InvestmentPositionId = position.Id,
                UserId = userId,
                UserId1 = userIdInt, // Set the actual FK to Users.Id
                TransactionType = InvestmentTransactionType.Create,
                Amount = request.Amount,
                Asset = "USDC",
                Status = InvestmentTransactionStatus.Confirmed
            };

            await _investmentRepository.CreateTransactionAsync(transaction);

            // Calculate projections
            var dailyReward = _rewardCalculation.CalculateDailyReward(request.Amount, apy);
            var monthlyReward = _rewardCalculation.CalculateProjectedReward(request.Amount, apy, 30);
            var yearlyReward = _rewardCalculation.CalculateProjectedReward(request.Amount, apy, 365);

            _logger.LogInformation("Created investment position {PositionId} for user {UserId}",
                position.Id, userId);

            return Ok(new CreateInvestmentResponse
            {
                InvestmentId = position.Id,
                PlanId = position.PlanId,
                Asset = position.Asset,
                Amount = position.PrincipalAmount,
                Apy = position.Apy,
                Status = position.Status.ToString(),
                EstimatedDailyReward = dailyReward,
                EstimatedMonthlyReward = monthlyReward,
                EstimatedYearlyReward = yearlyReward,
                CreatedAt = position.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create investment");
            return StatusCode(500, new { error = "Failed to create investment" });
        }
    }

    /// <summary>
    /// Get all investment positions for current user
    /// </summary>
    [HttpGet("positions")]
    [ProducesResponseType(typeof(List<InvestmentPositionResponse>), 200)]
    public async Task<IActionResult> GetPositions()
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

            var positions = await _investmentRepository.GetByUserIdAsync(userId);

            var response = positions.Select(p =>
            {
                var daysHeld = _rewardCalculation.CalculateDaysHeld(p.StartDate ?? p.CreatedAt);
                var dailyReward = _rewardCalculation.CalculateDailyReward(p.PrincipalAmount, p.Apy);
                var monthlyReward = _rewardCalculation.CalculateProjectedReward(p.PrincipalAmount, p.Apy, 30);
                var yearlyReward = _rewardCalculation.CalculateProjectedReward(p.PrincipalAmount, p.Apy, 365);

                return new InvestmentPositionResponse
                {
                    Id = p.Id,
                    PlanId = p.PlanId,
                    Asset = p.Asset,
                    PrincipalAmount = p.PrincipalAmount,
                    CurrentValue = p.CurrentValue,
                    AccruedRewards = p.AccruedRewards,
                    Apy = p.Apy,
                    Status = p.Status.ToString(),
                    StartDate = p.StartDate,
                    LastSyncedAt = p.LastSyncedAt,
                    DaysHeld = daysHeld,
                    EstimatedDailyReward = dailyReward,
                    EstimatedMonthlyReward = monthlyReward,
                    EstimatedYearlyReward = yearlyReward
                };
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get investment positions");
            return StatusCode(500, new { error = "Failed to retrieve positions" });
        }
    }

    /// <summary>
    /// Get detailed information for specific investment position
    /// </summary>
    [HttpGet("{id}/details")]
    [ProducesResponseType(typeof(InvestmentPositionDetailResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPositionDetails(Guid id)
    {
        try
        {
            var position = await _investmentRepository.GetByIdAsync(id);
            if (position == null)
            {
                return NotFound(new { error = "Investment position not found" });
            }

            var daysHeld = _rewardCalculation.CalculateDaysHeld(position.StartDate ?? position.CreatedAt);
            var dailyReward = _rewardCalculation.CalculateDailyReward(position.PrincipalAmount, position.Apy);
            var monthlyReward = _rewardCalculation.CalculateProjectedReward(position.PrincipalAmount, position.Apy, 30);
            var yearlyReward = _rewardCalculation.CalculateProjectedReward(position.PrincipalAmount, position.Apy, 365);

            var transactions = position.Transactions.Select(t => new InvestmentTransactionResponse
            {
                Id = t.Id,
                Type = t.TransactionType.ToString(),
                Amount = t.Amount,
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt
            }).ToList();

            var response = new InvestmentPositionDetailResponse
            {
                Id = position.Id,
                PlanId = position.PlanId,
                PlanName = $"USDC Flex Plan ({position.Apy}% APY)",
                Asset = position.Asset,
                PrincipalAmount = position.PrincipalAmount,
                CurrentValue = position.CurrentValue,
                AccruedRewards = position.AccruedRewards,
                Apy = position.Apy,
                Status = position.Status.ToString(),
                StartDate = position.StartDate,
                EndDate = position.EndDate,
                LastSyncedAt = position.LastSyncedAt,
                DaysHeld = daysHeld,
                EstimatedDailyReward = dailyReward,
                EstimatedMonthlyReward = monthlyReward,
                EstimatedYearlyReward = yearlyReward,
                Transactions = transactions,
                ProjectedRewards = new ProjectedRewardsResponse
                {
                    Daily = dailyReward,
                    Monthly = monthlyReward,
                    Yearly = yearlyReward
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get position details for {PositionId}", id);
            return StatusCode(500, new { error = "Failed to retrieve position details" });
        }
    }

    /// <summary>
    /// Withdraw investment position
    /// </summary>
    [HttpPost("{id}/withdraw")]
    [ProducesResponseType(typeof(WithdrawInvestmentResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> WithdrawInvestment(Guid id, [FromBody] WithdrawInvestmentRequest request)
    {
        try
        {
            var position = await _investmentRepository.GetByIdAsync(id);
            if (position == null)
            {
                return NotFound(new { error = "Investment position not found" });
            }

            if (position.Status != InvestmentStatus.Active)
            {
                return BadRequest(new { error = "Investment position is not active" });
            }

            // Get exchange connection
            var connection = await _connectionRepository.GetByIdAsync(position.ExchangeConnectionId);
            if (connection == null)
            {
                return BadRequest(new { error = "Exchange connection not found" });
            }

            // Decrypt credentials
            var apiKey = await _encryptionService.DecryptAsync(connection.ApiKeyEncrypted, position.UserId);
            var apiSecret = await _encryptionService.DecryptAsync(connection.ApiSecretEncrypted, position.UserId);

            // Close investment via WhiteBit API
            if (!string.IsNullOrEmpty(position.ExternalPositionId))
            {
                await _whiteBitClient.CloseInvestmentAsync(apiKey, apiSecret, position.ExternalPositionId);
            }

            // Update position status
            position.Status = InvestmentStatus.Closed;
            position.EndDate = DateTime.UtcNow;
            await _investmentRepository.UpdateAsync(position);

            // Create withdrawal transaction
            var transaction = new InvestmentTransaction
            {
                InvestmentPositionId = position.Id,
                UserId = position.UserId,
                UserId1 = position.UserId1, // Set the actual FK to Users.Id
                TransactionType = InvestmentTransactionType.Withdraw,
                Amount = position.CurrentValue,
                Asset = position.Asset,
                Status = InvestmentTransactionStatus.Confirmed
            };

            await _investmentRepository.CreateTransactionAsync(transaction);

            _logger.LogInformation("Withdrawn investment position {PositionId}", id);

            return Ok(new WithdrawInvestmentResponse
            {
                InvestmentId = position.Id,
                WithdrawalAmount = position.CurrentValue,
                Principal = position.PrincipalAmount,
                Rewards = position.AccruedRewards,
                Status = "processing",
                EstimatedCompletionTime = DateTime.UtcNow.AddMinutes(15)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to withdraw investment {PositionId}", id);
            return StatusCode(500, new { error = "Failed to withdraw investment" });
        }
    }
}
