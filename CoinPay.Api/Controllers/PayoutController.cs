using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoinPay.Api.DTOs;
using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using CoinPay.Api.Services.FiatGateway;
using CoinPay.Api.Services.BankAccount;
using CoinPay.Api.Services.Wallet;
using System.Security.Claims;

namespace CoinPay.Api.Controllers;

/// <summary>
/// Payout management endpoints for crypto-to-fiat withdrawals
/// </summary>
[ApiController]
[Route("api/payout")]
[Authorize]
public class PayoutController : ControllerBase
{
    private readonly IPayoutRepository _payoutRepository;
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IFiatGatewayService _fiatGatewayService;
    private readonly IWalletService _walletService;
    private readonly ILogger<PayoutController> _logger;

    public PayoutController(
        IPayoutRepository payoutRepository,
        IBankAccountRepository bankAccountRepository,
        IFiatGatewayService fiatGatewayService,
        IWalletService walletService,
        ILogger<PayoutController> logger)
    {
        _payoutRepository = payoutRepository;
        _bankAccountRepository = bankAccountRepository;
        _fiatGatewayService = fiatGatewayService;
        _walletService = walletService;
        _logger = logger;
    }

    /// <summary>
    /// Initiate payout to bank account
    /// </summary>
    [HttpPost("initiate")]
    [ProducesResponseType(typeof(PayoutResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PayoutResponse>> InitiatePayout([FromBody] InitiatePayoutRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("InitiatePayout: User ID not found in token");
            return Unauthorized(new { error = new { code = "UNAUTHORIZED", message = "User not authenticated" } });
        }

        try
        {
            // Verify bank account exists and belongs to user
            var bankAccount = await _bankAccountRepository.GetByIdAsync(request.BankAccountId);
            if (bankAccount == null || bankAccount.UserId != userId.Value)
            {
                _logger.LogWarning("InitiatePayout: Bank account {BankAccountId} not found for user {UserId}",
                    request.BankAccountId, userId);
                return BadRequest(new { error = new { code = "INVALID_BANK_ACCOUNT", message = "Bank account not found" } });
            }

            // Get user's wallet address
            var user = await _walletService.GetUserByIdAsync(userId.Value);
            if (user == null || string.IsNullOrEmpty(user.WalletAddress))
            {
                _logger.LogWarning("InitiatePayout: User {UserId} does not have a wallet", userId);
                return BadRequest(new { error = new { code = "NO_WALLET", message = "User does not have a wallet" } });
            }

            // Check USDC balance
            var balanceResult = await _walletService.GetWalletBalanceAsync(user.WalletAddress);
            if (balanceResult.USDCBalance < request.UsdcAmount)
            {
                _logger.LogWarning("InitiatePayout: Insufficient balance for user {UserId}. Required: {Required}, Available: {Available}",
                    userId, request.UsdcAmount, balanceResult.USDCBalance);
                return BadRequest(new { error = new { code = "INSUFFICIENT_BALANCE",
                    message = $"Insufficient USDC balance. Available: {balanceResult.USDCBalance} USDC" } });
            }

            // Decrypt bank account details
            var routingNumber = BankAccountEncryptionHelper.DecryptRoutingNumber(
                bankAccount.RoutingNumberEncrypted,
                HttpContext.RequestServices.GetRequiredService<Services.Encryption.IEncryptionService>());

            var accountNumber = BankAccountEncryptionHelper.DecryptAccountNumber(
                bankAccount.AccountNumberEncrypted,
                HttpContext.RequestServices.GetRequiredService<Services.Encryption.IEncryptionService>());

            // Initiate payout via gateway
            var gatewayRequest = new Services.FiatGateway.PayoutInitiationRequest
            {
                UserId = userId.Value,
                BankAccountId = request.BankAccountId,
                UsdcAmount = request.UsdcAmount,
                RoutingNumber = routingNumber,
                AccountNumber = accountNumber,
                AccountHolderName = bankAccount.AccountHolderName,
                AccountType = bankAccount.AccountType,
                BankName = bankAccount.BankName
            };

            var gatewayResponse = await _fiatGatewayService.InitiatePayoutAsync(gatewayRequest);

            if (!gatewayResponse.Success)
            {
                _logger.LogError("InitiatePayout: Gateway failed for user {UserId}. Error: {Error}",
                    userId, gatewayResponse.ErrorMessage);
                return BadRequest(new { error = new { code = gatewayResponse.ErrorCode ?? "GATEWAY_ERROR",
                    message = gatewayResponse.ErrorMessage ?? "Failed to initiate payout" } });
            }

            // Create payout record
            var payout = new PayoutTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                BankAccountId = request.BankAccountId,
                GatewayTransactionId = gatewayResponse.GatewayTransactionId,
                UsdcAmount = request.UsdcAmount,
                UsdAmount = gatewayResponse.UsdAmount,
                ExchangeRate = gatewayResponse.ExchangeRate,
                ConversionFee = gatewayResponse.TotalFees - 1.00m, // Subtract payout fee
                PayoutFee = 1.00m,
                TotalFees = gatewayResponse.TotalFees,
                NetAmount = gatewayResponse.NetAmount,
                Status = gatewayResponse.Status,
                InitiatedAt = DateTime.UtcNow,
                EstimatedArrival = gatewayResponse.EstimatedArrival
            };

            var created = await _payoutRepository.AddAsync(payout);

            _logger.LogInformation("InitiatePayout: Payout {PayoutId} created for user {UserId}, amount {Amount} USDC",
                created.Id, userId, request.UsdcAmount);

            // TODO: Deduct USDC from wallet (implement in future sprint)
            // await _walletService.DeductBalanceAsync(user.WalletAddress, request.UsdcAmount);

            var response = MapToPayoutResponse(created, bankAccount);
            return CreatedAtAction(nameof(GetPayoutStatus), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InitiatePayout: Error initiating payout for user {UserId}", userId);
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to initiate payout" } });
        }
    }

    /// <summary>
    /// Get payout history for authenticated user
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(PayoutHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PayoutHistoryResponse>> GetPayoutHistory(
        [FromQuery] int? limit = 20,
        [FromQuery] int? offset = 0)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { error = new { code = "UNAUTHORIZED", message = "User not authenticated" } });
        }

        try
        {
            var payouts = await _payoutRepository.GetAllByUserIdAsync(userId.Value, limit, offset);
            var total = await _payoutRepository.GetCountByUserIdAsync(userId.Value);

            var response = new PayoutHistoryResponse
            {
                Payouts = payouts.Select(p => MapToPayoutResponse(p, p.BankAccount!)).ToList(),
                Total = total,
                Offset = offset ?? 0,
                Limit = limit ?? 20
            };

            _logger.LogInformation("GetPayoutHistory: Retrieved {Count} payouts for user {UserId}", payouts.Count(), userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPayoutHistory: Error retrieving payout history for user {UserId}", userId);
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to retrieve payout history" } });
        }
    }

    /// <summary>
    /// Get payout status by ID
    /// </summary>
    [HttpGet("{id}/status")]
    [ProducesResponseType(typeof(DTOs.PayoutStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DTOs.PayoutStatusResponse>> GetPayoutStatus(Guid id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { error = new { code = "UNAUTHORIZED", message = "User not authenticated" } });
        }

        try
        {
            var payout = await _payoutRepository.GetByIdAsync(id);

            if (payout == null)
            {
                _logger.LogWarning("GetPayoutStatus: Payout {PayoutId} not found", id);
                return NotFound(new { error = new { code = "NOT_FOUND", message = "Payout not found" } });
            }

            // Verify ownership
            if (payout.UserId != userId.Value)
            {
                _logger.LogWarning("GetPayoutStatus: User {UserId} attempted to access payout {PayoutId} owned by {OwnerId}",
                    userId, id, payout.UserId);
                return NotFound(new { error = new { code = "NOT_FOUND", message = "Payout not found" } });
            }

            // Get latest status from gateway if still in progress
            if (payout.Status == "pending" || payout.Status == "processing")
            {
                if (!string.IsNullOrEmpty(payout.GatewayTransactionId))
                {
                    try
                    {
                        var gatewayStatus = await _fiatGatewayService.GetPayoutStatusAsync(payout.GatewayTransactionId);

                        // Update local status if changed
                        if (gatewayStatus != null && gatewayStatus.Status != payout.Status)
                        {
                            payout.Status = gatewayStatus.Status;
                            if (gatewayStatus.CompletedAt.HasValue)
                            {
                                payout.CompletedAt = gatewayStatus.CompletedAt;
                            }
                            if (!string.IsNullOrEmpty(gatewayStatus.FailureReason))
                            {
                                payout.FailureReason = gatewayStatus.FailureReason;
                            }

                            await _payoutRepository.UpdateAsync(payout);
                            _logger.LogInformation("GetPayoutStatus: Updated payout {PayoutId} status to {Status}", id, payout.Status);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "GetPayoutStatus: Failed to fetch gateway status for payout {PayoutId}", id);
                        // Continue with local status if gateway fails
                    }
                }
            }

            var response = MapToPayoutStatusResponse(payout);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPayoutStatus: Error retrieving status for payout {PayoutId}", id);
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to retrieve payout status" } });
        }
    }

    /// <summary>
    /// Get conversion preview (calculate fees without initiating payout)
    /// </summary>
    [HttpPost("preview")]
    [ProducesResponseType(typeof(DTOs.ConversionPreviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DTOs.ConversionPreviewResponse>> GetConversionPreview([FromBody] ConversionPreviewRequest request)
    {
        try
        {
            var preview = await _fiatGatewayService.GetConversionPreviewAsync(request.UsdcAmount);

            var response = new DTOs.ConversionPreviewResponse
            {
                UsdcAmount = preview.UsdcAmount,
                ExchangeRate = preview.ExchangeRate,
                UsdAmountBeforeFees = preview.UsdAmountBeforeFees,
                ConversionFeePercent = preview.ConversionFeePercent,
                ConversionFeeAmount = preview.ConversionFeeAmount,
                PayoutFeeAmount = preview.PayoutFeeAmount,
                TotalFees = preview.TotalFees,
                NetUsdAmount = preview.NetUsdAmount,
                ExpiresAt = preview.ExpiresAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetConversionPreview: Error calculating preview for amount {Amount}", request.UsdcAmount);
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to calculate conversion preview" } });
        }
    }

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }
        return null;
    }

    private static PayoutResponse MapToPayoutResponse(PayoutTransaction payout, BankAccount bankAccount)
    {
        return new PayoutResponse
        {
            Id = payout.Id,
            BankAccountId = payout.BankAccountId,
            GatewayTransactionId = payout.GatewayTransactionId,
            UsdcAmount = payout.UsdcAmount,
            UsdAmount = payout.UsdAmount,
            ExchangeRate = payout.ExchangeRate,
            ConversionFee = payout.ConversionFee,
            PayoutFee = payout.PayoutFee,
            TotalFees = payout.TotalFees,
            NetAmount = payout.NetAmount,
            Status = payout.Status,
            InitiatedAt = payout.InitiatedAt,
            CompletedAt = payout.CompletedAt,
            EstimatedArrival = payout.EstimatedArrival,
            FailureReason = payout.FailureReason,
            BankAccount = new BankAccountSummary
            {
                Id = bankAccount.Id,
                AccountHolderName = bankAccount.AccountHolderName,
                LastFourDigits = bankAccount.LastFourDigits,
                AccountType = bankAccount.AccountType,
                BankName = bankAccount.BankName
            }
        };
    }

    private static DTOs.PayoutStatusResponse MapToPayoutStatusResponse(PayoutTransaction payout)
    {
        var stage = payout.Status switch
        {
            "pending" => "initiated",
            "processing" => "converting",
            "completed" => "completed",
            "failed" => "failed",
            _ => "initiated"
        };

        var events = new List<DTOs.PayoutStatusEvent>
        {
            new DTOs.PayoutStatusEvent
            {
                Event = "INITIATED",
                Timestamp = payout.InitiatedAt,
                Description = "Payout initiated"
            }
        };

        if (payout.Status == "processing" || payout.Status == "completed")
        {
            events.Add(new DTOs.PayoutStatusEvent
            {
                Event = "PROCESSING",
                Timestamp = payout.InitiatedAt.AddSeconds(10),
                Description = "Converting USDC to USD"
            });
        }

        if (payout.Status == "completed" && payout.CompletedAt.HasValue)
        {
            events.Add(new DTOs.PayoutStatusEvent
            {
                Event = "COMPLETED",
                Timestamp = payout.CompletedAt.Value,
                Description = "Payout completed successfully"
            });
        }

        if (payout.Status == "failed")
        {
            events.Add(new DTOs.PayoutStatusEvent
            {
                Event = "FAILED",
                Timestamp = payout.CompletedAt ?? DateTime.UtcNow,
                Description = payout.FailureReason ?? "Payout failed"
            });
        }

        return new DTOs.PayoutStatusResponse
        {
            Id = payout.Id,
            Status = payout.Status,
            Stage = stage,
            InitiatedAt = payout.InitiatedAt,
            CompletedAt = payout.CompletedAt,
            EstimatedArrival = payout.EstimatedArrival,
            FailureReason = payout.FailureReason,
            LastUpdated = payout.CompletedAt ?? payout.InitiatedAt,
            Events = events
        };
    }
}
