using CoinPay.Api.DTOs;
using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using CoinPay.Api.Services.Caching;
using CoinPay.Api.Services.Swap;
using CoinPay.Api.Services.Swap.Exceptions;
using CoinPay.Api.Services.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoinPay.Api.Controllers;

/// <summary>
/// Controller for token swap operations
/// </summary>
[ApiController]
[Route("api/swap")]
[Produces("application/json")]
public class SwapController : ControllerBase
{
    private readonly ISwapQuoteService _quoteService;
    private readonly ISwapExecutionService _executionService;
    private readonly ISwapTransactionRepository _swapRepository;
    private readonly ISwapQuoteCacheService _cacheService;
    private readonly IWalletService _walletService;
    private readonly ILogger<SwapController> _logger;

    public SwapController(
        ISwapQuoteService quoteService,
        ISwapExecutionService executionService,
        ISwapTransactionRepository swapRepository,
        ISwapQuoteCacheService cacheService,
        IWalletService walletService,
        ILogger<SwapController> logger)
    {
        _quoteService = quoteService;
        _executionService = executionService;
        _swapRepository = swapRepository;
        _cacheService = cacheService;
        _walletService = walletService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a swap quote with fees and price impact
    /// </summary>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    /// <param name="amount">Amount to swap</param>
    /// <param name="slippage">Slippage tolerance (default: 1.0%)</param>
    /// <returns>Swap quote with exchange rate and fees</returns>
    [HttpGet("quote")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SwapQuoteResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetQuote(
        [FromQuery] string fromToken,
        [FromQuery] string toToken,
        [FromQuery] decimal amount,
        [FromQuery] decimal slippage = 1.0m)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(fromToken) || string.IsNullOrWhiteSpace(toToken))
            {
                return BadRequest(new { error = "Token addresses are required" });
            }

            if (amount <= 0)
            {
                return BadRequest(new { error = "Amount must be greater than 0" });
            }

            if (slippage < 0.1m || slippage > 50m)
            {
                return BadRequest(new { error = "Slippage must be between 0.1% and 50%" });
            }

            // Check cache first
            var cachedQuote = await _cacheService.GetCachedQuoteAsync(
                fromToken,
                toToken,
                amount,
                slippage);

            if (cachedQuote != null)
            {
                _logger.LogInformation("Returning cached quote for {From} -> {To}", fromToken, toToken);
                return Ok(MapToQuoteResponse(cachedQuote));
            }

            // Get fresh quote
            var quote = await _quoteService.GetBestQuoteAsync(
                fromToken,
                toToken,
                amount,
                slippage);

            // Cache the quote
            await _cacheService.SetCachedQuoteAsync(quote, fromToken, toToken, amount, slippage);

            var response = MapToQuoteResponse(quote);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid quote request");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting swap quote");
            return StatusCode(500, new { error = "Failed to get swap quote" });
        }
    }

    /// <summary>
    /// Executes a token swap
    /// </summary>
    /// <param name="request">Swap execution request</param>
    /// <returns>Swap execution result with transaction hash</returns>
    [HttpPost("execute")]
    [Authorize]
    [ProducesResponseType(typeof(SwapExecutionResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ExecuteSwap([FromBody] ExecuteSwapRequest request)
    {
        try
        {
            // Get user ID from JWT
            var userId = GetUserIdFromClaims();
            if (userId == Guid.Empty)
            {
                return Unauthorized(new { error = "Invalid user token" });
            }

            // Get user's wallet address
            var wallet = await _walletService.GetUserWalletAsync(userId);
            if (wallet == null)
            {
                return BadRequest(new { error = "Wallet not found. Please create a wallet first." });
            }

            // Validate inputs
            if (string.IsNullOrWhiteSpace(request.FromToken) ||
                string.IsNullOrWhiteSpace(request.ToToken))
            {
                return BadRequest(new { error = "Token addresses are required" });
            }

            if (request.FromAmount <= 0)
            {
                return BadRequest(new { error = "Amount must be greater than 0" });
            }

            if (request.SlippageTolerance < 0.1m || request.SlippageTolerance > 50m)
            {
                return BadRequest(new { error = "Slippage must be between 0.1% and 50%" });
            }

            // Execute swap
            var result = await _executionService.ExecuteSwapAsync(
                userId,
                wallet.Address,
                request.FromToken,
                request.ToToken,
                request.FromAmount,
                request.SlippageTolerance);

            var response = MapToExecutionResponse(result, request);

            _logger.LogInformation(
                "Swap executed: SwapId={SwapId}, User={UserId}, TxHash={TxHash}",
                result.SwapId,
                userId,
                result.TransactionHash);

            return Ok(response);
        }
        catch (InsufficientBalanceException ex)
        {
            _logger.LogWarning(ex, "Insufficient balance for swap");
            return BadRequest(new
            {
                error = "Insufficient balance",
                details = new
                {
                    required = ex.Required,
                    available = ex.Available,
                    shortfall = ex.Shortfall
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid swap request");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing swap");
            return StatusCode(500, new { error = "Failed to execute swap" });
        }
    }

    /// <summary>
    /// Gets swap transaction history for the authenticated user
    /// </summary>
    /// <param name="status">Filter by status (all, pending, confirmed, failed)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20)</param>
    /// <param name="sortBy">Sort field (createdAt, fromAmount)</param>
    /// <param name="sortOrder">Sort order (desc, asc)</param>
    /// <returns>Paginated swap history</returns>
    [HttpGet("history")]
    [Authorize]
    [ProducesResponseType(typeof(SwapHistoryResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetSwapHistory(
        [FromQuery] string status = "all",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc")
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == Guid.Empty)
            {
                return Unauthorized(new { error = "Invalid user token" });
            }

            // Parse status filter
            SwapStatus? statusFilter = status.ToLower() switch
            {
                "pending" => SwapStatus.Pending,
                "confirmed" => SwapStatus.Confirmed,
                "failed" => SwapStatus.Failed,
                _ => null
            };

            // Get swaps
            var swaps = await _swapRepository.GetByUserIdAsync(
                userId,
                page,
                pageSize,
                statusFilter);

            // Get total count
            var totalCount = await _swapRepository.GetSwapCountByUserAsync(userId, statusFilter);

            var response = new SwapHistoryResponse
            {
                Swaps = swaps.Select(MapToHistoryItem).ToList(),
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting swap history");
            return StatusCode(500, new { error = "Failed to get swap history" });
        }
    }

    /// <summary>
    /// Gets detailed information for a specific swap
    /// </summary>
    /// <param name="id">Swap transaction ID</param>
    /// <returns>Detailed swap information</returns>
    [HttpGet("{id}/details")]
    [Authorize]
    [ProducesResponseType(typeof(SwapDetailsResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetSwapDetails([FromRoute] Guid id)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == Guid.Empty)
            {
                return Unauthorized(new { error = "Invalid user token" });
            }

            var swap = await _swapRepository.GetByIdAsync(id);

            if (swap == null)
            {
                return NotFound(new { error = "Swap not found" });
            }

            // Verify swap belongs to user
            if (swap.UserId != userId)
            {
                return NotFound(new { error = "Swap not found" });
            }

            var response = MapToDetailsResponse(swap);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting swap details");
            return StatusCode(500, new { error = "Failed to get swap details" });
        }
    }

    // Helper methods
    private Guid GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return Guid.Empty;
    }

    private SwapQuoteResponse MapToQuoteResponse(SwapQuoteResult quote)
    {
        return new SwapQuoteResponse
        {
            FromToken = quote.FromToken,
            FromTokenSymbol = quote.FromTokenSymbol,
            ToToken = quote.ToToken,
            ToTokenSymbol = quote.ToTokenSymbol,
            FromAmount = quote.FromAmount,
            ToAmount = quote.ToAmount,
            ExchangeRate = quote.ExchangeRate,
            PlatformFee = quote.PlatformFee,
            PlatformFeePercentage = quote.PlatformFeePercentage,
            EstimatedGas = quote.EstimatedGas,
            EstimatedGasCost = quote.EstimatedGasCost,
            PriceImpact = quote.PriceImpact,
            SlippageTolerance = quote.SlippageTolerance,
            MinimumReceived = quote.MinimumReceived,
            QuoteValidUntil = quote.QuoteValidUntil,
            Provider = quote.Provider
        };
    }

    private SwapExecutionResponse MapToExecutionResponse(
        SwapExecutionResult result,
        ExecuteSwapRequest request)
    {
        return new SwapExecutionResponse
        {
            SwapId = result.SwapId,
            TransactionHash = result.TransactionHash,
            Status = result.Status.ToString().ToLower(),
            FromToken = request.FromToken,
            FromTokenSymbol = Services.Swap.OneInch.TestnetTokens.GetSymbol(request.FromToken),
            ToToken = request.ToToken,
            ToTokenSymbol = Services.Swap.OneInch.TestnetTokens.GetSymbol(request.ToToken),
            FromAmount = request.FromAmount,
            ExpectedToAmount = result.ExpectedToAmount,
            MinimumReceived = result.MinimumReceived,
            PlatformFee = result.PlatformFee,
            EstimatedConfirmationTime = "30-60 seconds",
            Message = result.Message
        };
    }

    private SwapHistoryItem MapToHistoryItem(SwapTransaction swap)
    {
        return new SwapHistoryItem
        {
            Id = swap.Id,
            FromToken = swap.FromToken,
            FromTokenSymbol = swap.FromTokenSymbol,
            ToToken = swap.ToToken,
            ToTokenSymbol = swap.ToTokenSymbol,
            FromAmount = swap.FromAmount,
            ToAmount = swap.ToAmount,
            ExchangeRate = swap.ExchangeRate,
            PlatformFee = swap.PlatformFee,
            Status = swap.Status.ToString().ToLower(),
            TransactionHash = swap.TransactionHash,
            CreatedAt = swap.CreatedAt,
            ConfirmedAt = swap.ConfirmedAt
        };
    }

    private SwapDetailsResponse MapToDetailsResponse(SwapTransaction swap)
    {
        return new SwapDetailsResponse
        {
            Id = swap.Id,
            FromToken = swap.FromToken,
            FromTokenSymbol = swap.FromTokenSymbol,
            ToToken = swap.ToToken,
            ToTokenSymbol = swap.ToTokenSymbol,
            FromAmount = swap.FromAmount,
            ToAmount = swap.ToAmount,
            ExchangeRate = swap.ExchangeRate,
            PlatformFee = swap.PlatformFee,
            PlatformFeePercentage = swap.PlatformFeePercentage,
            GasUsed = swap.GasUsed,
            GasCost = swap.GasCost,
            SlippageTolerance = swap.SlippageTolerance,
            PriceImpact = swap.PriceImpact,
            MinimumReceived = swap.MinimumReceived,
            DexProvider = swap.DexProvider,
            TransactionHash = swap.TransactionHash,
            Status = swap.Status.ToString().ToLower(),
            ErrorMessage = swap.ErrorMessage,
            CreatedAt = swap.CreatedAt,
            ConfirmedAt = swap.ConfirmedAt
        };
    }
}

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
}
