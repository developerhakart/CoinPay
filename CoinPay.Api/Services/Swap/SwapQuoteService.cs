using CoinPay.Api.Models;
using CoinPay.Api.Services.Swap.OneInch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Service to fetch and compare swap quotes with fee calculations
/// </summary>
public class SwapQuoteService : ISwapQuoteService
{
    private readonly IDexAggregatorService _dexService;
    private readonly IFeeCalculationService _feeService;
    private readonly ISlippageToleranceService _slippageService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SwapQuoteService> _logger;

    private int QuoteTtlSeconds => _configuration.GetValue<int>("Swap:CacheTTLSeconds", 30);

    public SwapQuoteService(
        IDexAggregatorService dexService,
        IFeeCalculationService feeService,
        ISlippageToleranceService slippageService,
        IConfiguration configuration,
        ILogger<SwapQuoteService> logger)
    {
        _dexService = dexService;
        _feeService = feeService;
        _slippageService = slippageService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SwapQuoteResult> GetBestQuoteAsync(
        string fromToken,
        string toToken,
        decimal fromAmount,
        decimal slippageTolerance)
    {
        _logger.LogInformation(
            "Getting best swap quote: {FromToken} -> {ToToken}, Amount: {Amount}, Slippage: {Slippage}%",
            fromToken,
            toToken,
            fromAmount,
            slippageTolerance);

        // Validate token pair
        await ValidateTokenPairAsync(fromToken, toToken);

        // Validate slippage
        _slippageService.ValidateSlippage(slippageTolerance);

        // Get quote from DEX aggregator
        var dexQuote = await _dexService.GetQuoteAsync(
            fromToken,
            toToken,
            fromAmount,
            slippageTolerance);

        // Calculate platform fee
        var platformFee = await _feeService.CalculateSwapFeeAsync(
            fromToken,
            fromAmount);

        var feePercentage = await _feeService.GetFeePercentageAsync();

        // Calculate price impact
        var priceImpact = CalculatePriceImpact(
            fromAmount,
            dexQuote.ToTokenAmount,
            dexQuote.ExchangeRate);

        // Calculate minimum received after slippage
        var minimumReceived = _slippageService.CalculateMinimumReceived(
            dexQuote.ToTokenAmount,
            slippageTolerance);

        // Estimate gas cost
        var estimatedGasCost = await EstimateGasCostAsync(dexQuote.EstimatedGas);

        // Build complete quote result
        var quoteResult = new SwapQuoteResult
        {
            FromToken = fromToken,
            FromTokenSymbol = TestnetTokens.GetSymbol(fromToken),
            ToToken = toToken,
            ToTokenSymbol = TestnetTokens.GetSymbol(toToken),
            FromAmount = fromAmount,
            ToAmount = dexQuote.ToTokenAmount,
            ExchangeRate = dexQuote.ExchangeRate,
            PlatformFee = platformFee,
            PlatformFeePercentage = feePercentage,
            EstimatedGas = dexQuote.EstimatedGas,
            EstimatedGasCost = estimatedGasCost,
            PriceImpact = priceImpact,
            SlippageTolerance = slippageTolerance,
            MinimumReceived = minimumReceived,
            QuoteValidUntil = DateTime.UtcNow.AddSeconds(QuoteTtlSeconds),
            Provider = dexQuote.Provider
        };

        _logger.LogInformation(
            "Quote result: {FromAmount} {FromSymbol} = {ToAmount} {ToSymbol}, Rate: {Rate}, Fee: {Fee} {FromSymbol}, Impact: {Impact}%",
            quoteResult.FromAmount,
            quoteResult.FromTokenSymbol,
            quoteResult.ToAmount,
            quoteResult.ToTokenSymbol,
            quoteResult.ExchangeRate,
            quoteResult.PlatformFee,
            quoteResult.FromTokenSymbol,
            quoteResult.PriceImpact);

        return quoteResult;
    }

    public Task<bool> ValidateTokenPairAsync(string fromToken, string toToken)
    {
        if (string.IsNullOrWhiteSpace(fromToken))
        {
            throw new InvalidOperationException("Source token address is required");
        }

        if (string.IsNullOrWhiteSpace(toToken))
        {
            throw new InvalidOperationException("Destination token address is required");
        }

        if (fromToken.Equals(toToken, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Cannot swap a token for itself");
        }

        // Validate tokens are supported
        var fromSymbol = TestnetTokens.GetSymbol(fromToken);
        var toSymbol = TestnetTokens.GetSymbol(toToken);

        if (fromSymbol == "UNKNOWN" || toSymbol == "UNKNOWN")
        {
            throw new InvalidOperationException(
                $"Unsupported token pair: {fromToken} -> {toToken}. Only USDC, WETH, WMATIC are supported.");
        }

        _logger.LogDebug("Token pair validated: {FromSymbol} -> {ToSymbol}", fromSymbol, toSymbol);

        return Task.FromResult(true);
    }

    private decimal CalculatePriceImpact(
        decimal fromAmount,
        decimal toAmount,
        decimal exchangeRate)
    {
        if (fromAmount <= 0 || exchangeRate <= 0)
        {
            return 0;
        }

        // Expected output at current rate
        var expectedToAmount = fromAmount * exchangeRate;

        // Price impact = (Expected - Actual) / Expected * 100
        var priceImpact = expectedToAmount > 0
            ? Math.Abs((expectedToAmount - toAmount) / expectedToAmount * 100)
            : 0;

        // Round to 2 decimal places
        priceImpact = Math.Round(priceImpact, 2);

        if (priceImpact > 1.0m)
        {
            _logger.LogWarning(
                "High price impact detected: {PriceImpact}% for {Amount} swap",
                priceImpact,
                fromAmount);
        }

        return priceImpact;
    }

    private Task<decimal> EstimateGasCostAsync(string estimatedGas)
    {
        if (!long.TryParse(estimatedGas, out var gasUnits))
        {
            return Task.FromResult(0.01m); // Default estimate
        }

        // Typical gas price on Polygon Amoy is ~30 gwei
        var gasPriceGwei = 30m;
        var gasCostMatic = (gasUnits * gasPriceGwei) / 1_000_000_000m; // Convert to MATIC

        // Round to 6 decimal places
        gasCostMatic = Math.Round(gasCostMatic, 6);

        return Task.FromResult(gasCostMatic);
    }
}
