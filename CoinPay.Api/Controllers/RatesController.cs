using Microsoft.AspNetCore.Mvc;
using CoinPay.Api.Services.ExchangeRate;
using CoinPay.Api.Services.Fees;

namespace CoinPay.Api.Controllers;

/// <summary>
/// Exchange rates and fee information endpoints
/// Provides real-time rates and fee structure to frontend
/// </summary>
[ApiController]
[Route("api/rates")]
public class RatesController : ControllerBase
{
    private readonly IExchangeRateService _exchangeRateService;
    private readonly IConversionFeeCalculator _feeCalculator;
    private readonly ILogger<RatesController> _logger;

    public RatesController(
        IExchangeRateService exchangeRateService,
        IConversionFeeCalculator feeCalculator,
        ILogger<RatesController> logger)
    {
        _exchangeRateService = exchangeRateService;
        _feeCalculator = feeCalculator;
        _logger = logger;
    }

    /// <summary>
    /// Get current USDC to USD exchange rate
    /// </summary>
    /// <remarks>
    /// Returns real-time exchange rate with caching (30-second TTL).
    /// Rate includes metadata about source, validity period, and expiration.
    /// Frontend should refresh when SecondsUntilExpiration reaches 0.
    ///
    /// Example Response:
    /// ```json
    /// {
    ///   "rate": 0.9998,
    ///   "baseCurrency": "USDC",
    ///   "quoteCurrency": "USD",
    ///   "timestamp": "2025-01-15T10:30:00Z",
    ///   "validForSeconds": 30,
    ///   "source": "Mock",
    ///   "isCached": true,
    ///   "expiresAt": "2025-01-15T10:30:30Z",
    ///   "isValid": true,
    ///   "secondsUntilExpiration": 25
    /// }
    /// ```
    /// </remarks>
    /// <returns>Current exchange rate information</returns>
    [HttpGet("usdc-usd")]
    [ProducesResponseType(typeof(ExchangeRateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ExchangeRateResponse>> GetUsdcToUsdRate()
    {
        try
        {
            _logger.LogInformation("GET /api/rates/usdc-usd - Fetching exchange rate");

            var rateInfo = await _exchangeRateService.GetUsdcToUsdRateAsync();

            var response = new ExchangeRateResponse
            {
                Rate = rateInfo.Rate,
                BaseCurrency = rateInfo.BaseCurrency,
                QuoteCurrency = rateInfo.QuoteCurrency,
                Timestamp = rateInfo.Timestamp,
                ValidForSeconds = rateInfo.ValidForSeconds,
                Source = rateInfo.Source,
                IsCached = rateInfo.IsCached,
                ExpiresAt = rateInfo.ExpiresAt,
                IsValid = rateInfo.IsValid,
                SecondsUntilExpiration = rateInfo.SecondsUntilExpiration
            };

            _logger.LogInformation("Exchange rate retrieved: {Rate} {BaseCurrency}/{QuoteCurrency} from {Source} (cached: {IsCached})",
                response.Rate, response.BaseCurrency, response.QuoteCurrency, response.Source, response.IsCached);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch exchange rate");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                error = new
                {
                    code = "RATE_SERVICE_UNAVAILABLE",
                    message = "Exchange rate service is temporarily unavailable"
                }
            });
        }
    }

    /// <summary>
    /// Get fee configuration and structure
    /// </summary>
    /// <remarks>
    /// Returns current fee structure including conversion fee percentage and payout flat fee.
    /// Useful for displaying fee transparency to users.
    ///
    /// Example Response:
    /// ```json
    /// {
    ///   "conversionFeePercent": 1.5,
    ///   "payoutFlatFee": 1.00,
    ///   "minimumPayoutAmount": 10.00,
    ///   "maximumPayoutAmount": null,
    ///   "feeTier": "Standard",
    ///   "description": "1.5% conversion fee + $1 payout fee"
    /// }
    /// ```
    /// </remarks>
    /// <returns>Current fee configuration</returns>
    [HttpGet("fees")]
    [ProducesResponseType(typeof(FeeConfiguration), StatusCodes.Status200OK)]
    public ActionResult<FeeConfiguration> GetFeeConfiguration()
    {
        _logger.LogInformation("GET /api/rates/fees - Fetching fee configuration");

        var config = _feeCalculator.GetFeeConfiguration();

        _logger.LogInformation("Fee configuration retrieved: {Description}", config.Description);

        return Ok(config);
    }

    /// <summary>
    /// Calculate fee breakdown for a specific amount
    /// </summary>
    /// <param name="usdAmount">USD amount to calculate fees for</param>
    /// <remarks>
    /// Calculates detailed fee breakdown for a given USD amount.
    /// Useful for showing users exactly what they'll pay in fees.
    ///
    /// Example Request:
    /// ```
    /// GET /api/rates/fees/calculate?usdAmount=100.00
    /// ```
    ///
    /// Example Response:
    /// ```json
    /// {
    ///   "usdAmountBeforeFees": 100.00,
    ///   "conversionFeePercent": 1.5,
    ///   "conversionFeeAmount": 1.50,
    ///   "payoutFeeAmount": 1.00,
    ///   "totalFees": 2.50,
    ///   "netAmount": 97.50,
    ///   "effectiveFeeRate": 2.5
    /// }
    /// ```
    /// </remarks>
    /// <returns>Detailed fee breakdown</returns>
    [HttpGet("fees/calculate")]
    [ProducesResponseType(typeof(FeeBreakdown), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<FeeBreakdown> CalculateFees([FromQuery] decimal usdAmount)
    {
        if (usdAmount <= 0)
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "INVALID_AMOUNT",
                    message = "Amount must be greater than 0"
                }
            });
        }

        _logger.LogInformation("GET /api/rates/fees/calculate?usdAmount={UsdAmount}", usdAmount);

        var breakdown = _feeCalculator.CalculateConversionFees(usdAmount);

        return Ok(breakdown);
    }

    /// <summary>
    /// Health check for exchange rate service
    /// </summary>
    /// <returns>Service availability status</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(RateServiceHealthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RateServiceHealthResponse>> CheckHealth()
    {
        _logger.LogDebug("GET /api/rates/health - Health check");

        var isAvailable = await _exchangeRateService.IsAvailableAsync();

        var response = new RateServiceHealthResponse
        {
            IsAvailable = isAvailable,
            Status = isAvailable ? "healthy" : "unhealthy",
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }

    /// <summary>
    /// Force refresh exchange rate (admin/debug endpoint)
    /// </summary>
    /// <returns>Newly fetched exchange rate</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ExchangeRateResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExchangeRateResponse>> RefreshRate()
    {
        _logger.LogInformation("POST /api/rates/refresh - Forcing rate refresh");

        var rateInfo = await _exchangeRateService.RefreshRateAsync();

        var response = new ExchangeRateResponse
        {
            Rate = rateInfo.Rate,
            BaseCurrency = rateInfo.BaseCurrency,
            QuoteCurrency = rateInfo.QuoteCurrency,
            Timestamp = rateInfo.Timestamp,
            ValidForSeconds = rateInfo.ValidForSeconds,
            Source = rateInfo.Source,
            IsCached = false,
            ExpiresAt = rateInfo.ExpiresAt,
            IsValid = rateInfo.IsValid,
            SecondsUntilExpiration = rateInfo.SecondsUntilExpiration
        };

        return Ok(response);
    }
}

#region DTOs

/// <summary>
/// Exchange rate API response
/// </summary>
public class ExchangeRateResponse
{
    public decimal Rate { get; set; }
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int ValidForSeconds { get; set; }
    public string Source { get; set; } = string.Empty;
    public bool IsCached { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsValid { get; set; }
    public int SecondsUntilExpiration { get; set; }
}

/// <summary>
/// Rate service health check response
/// </summary>
public class RateServiceHealthResponse
{
    public bool IsAvailable { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

#endregion
