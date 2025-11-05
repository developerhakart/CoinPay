using CoinPay.Api.Models;
using CoinPay.Api.Services.Swap.OneInch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Net;
using System.Text.Json;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// 1inch DEX aggregator service implementation
/// </summary>
public class OneInchAggregatorService : IDexAggregatorService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OneInchAggregatorService> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;

    private const string PROVIDER_NAME = "1inch";
    private static readonly SemaphoreSlim _rateLimiter = new(10, 10); // 10 requests per second max
    private static readonly TimeSpan _rateLimitPeriod = TimeSpan.FromSeconds(1);

    private string BaseUrl => _configuration["OneInch:ApiBaseUrl"] ?? "https://api.1inch.io/v5.0";
    private int ChainId => _configuration.GetValue<int>("OneInch:ChainId", 80002); // Polygon Amoy testnet
    private string? ApiKey => _configuration["OneInch:ApiKey"];
    private bool UseMockMode => _configuration.GetValue<bool>("OneInch:UseMockMode", false);

    public OneInchAggregatorService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OneInchAggregatorService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;

        // Configure retry policy with exponential backoff
        _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests ||
                          r.StatusCode == HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "1inch API request failed. Retry {RetryCount} after {Delay}ms. Status: {StatusCode}",
                        retryCount,
                        timespan.TotalMilliseconds,
                        outcome.Result?.StatusCode);
                });

        // Configure circuit breaker
        _circuitBreakerPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => r.StatusCode == HttpStatusCode.ServiceUnavailable)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromMinutes(1),
                onBreak: (outcome, duration) =>
                {
                    _logger.LogError("1inch API circuit breaker opened for {Duration}s", duration.TotalSeconds);
                },
                onReset: () =>
                {
                    _logger.LogInformation("1inch API circuit breaker reset");
                });
    }

    public async Task<SwapQuote> GetQuoteAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippageTolerance)
    {
        if (UseMockMode)
        {
            return GetMockQuote(fromToken, toToken, amount);
        }

        _logger.LogInformation(
            "Fetching 1inch quote: {FromToken} -> {ToToken}, Amount: {Amount}, Slippage: {Slippage}%",
            fromToken, toToken, amount, slippageTolerance);

        var amountInWei = ConvertToWei(amount, fromToken);
        var endpoint = $"{BaseUrl}/{ChainId}/quote";

        var queryParams = new Dictionary<string, string>
        {
            ["fromTokenAddress"] = fromToken,
            ["toTokenAddress"] = toToken,
            ["amount"] = amountInWei.ToString()
        };

        var response = await SendRequestAsync<OneInchQuoteResponse>(endpoint, queryParams);

        var swapQuote = MapToSwapQuote(response, fromToken, toToken);

        _logger.LogInformation(
            "1inch quote fetched: {FromAmount} {FromSymbol} = {ToAmount} {ToSymbol}, Rate: {Rate}",
            amount,
            TestnetTokens.GetSymbol(fromToken),
            swapQuote.ToTokenAmount,
            TestnetTokens.GetSymbol(toToken),
            swapQuote.ExchangeRate);

        return swapQuote;
    }

    public async Task<DexSwapTransaction> GetSwapTransactionAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippageTolerance,
        string fromAddress)
    {
        if (UseMockMode)
        {
            return GetMockSwapTransaction(fromToken, toToken, amount, slippageTolerance, fromAddress);
        }

        _logger.LogInformation(
            "Fetching 1inch swap transaction: {FromToken} -> {ToToken}, Amount: {Amount}, From: {FromAddress}",
            fromToken, toToken, amount, fromAddress);

        var amountInWei = ConvertToWei(amount, fromToken);
        var endpoint = $"{BaseUrl}/{ChainId}/swap";

        var queryParams = new Dictionary<string, string>
        {
            ["fromTokenAddress"] = fromToken,
            ["toTokenAddress"] = toToken,
            ["amount"] = amountInWei.ToString(),
            ["fromAddress"] = fromAddress,
            ["slippage"] = slippageTolerance.ToString("F1"),
            ["disableEstimate"] = "false"
        };

        var response = await SendRequestAsync<OneInchSwapResponse>(endpoint, queryParams);

        var swapTx = MapToSwapTransaction(response, fromAddress);

        _logger.LogInformation(
            "1inch swap transaction fetched: To={To}, Gas={Gas}",
            swapTx.To,
            swapTx.Gas);

        return swapTx;
    }

    public Task<decimal> EstimateGasAsync(DexSwapTransaction swapTx)
    {
        // Simple estimation: gas units * typical gas price
        if (long.TryParse(swapTx.Gas, out var gasUnits))
        {
            // Typical gas price on Polygon is ~30 gwei
            var gasPriceGwei = 30m;
            var gasCostMatic = (gasUnits * gasPriceGwei) / 1_000_000_000m; // Convert to MATIC
            return Task.FromResult(gasCostMatic);
        }

        return Task.FromResult(0.01m); // Fallback estimate
    }

    private async Task<T> SendRequestAsync<T>(string endpoint, Dictionary<string, string> queryParams)
    {
        // Rate limiting
        await _rateLimiter.WaitAsync();
        _ = Task.Run(async () =>
        {
            await Task.Delay(_rateLimitPeriod);
            _rateLimiter.Release();
        });

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var url = $"{endpoint}?{queryString}";

        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        if (!string.IsNullOrEmpty(ApiKey))
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        }

        var response = await _retryPolicy.ExecuteAsync(async () =>
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var httpResponse = await client.GetAsync(url);
                httpResponse.EnsureSuccessStatusCode();
                return httpResponse;
            });
        });

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null)
        {
            throw new InvalidOperationException("Failed to deserialize 1inch API response");
        }

        return result;
    }

    private string ConvertToWei(decimal amount, string tokenAddress)
    {
        var decimals = TestnetTokens.GetDecimals(tokenAddress);
        var multiplier = (decimal)Math.Pow(10, decimals);
        var amountInWei = amount * multiplier;
        return amountInWei.ToString("F0");
    }

    private decimal ConvertFromWei(string amountWei, string tokenAddress)
    {
        if (!decimal.TryParse(amountWei, out var wei))
        {
            return 0;
        }

        var decimals = TestnetTokens.GetDecimals(tokenAddress);
        var divisor = (decimal)Math.Pow(10, decimals);
        return wei / divisor;
    }

    private SwapQuote MapToSwapQuote(OneInchQuoteResponse response, string fromToken, string toToken)
    {
        var fromAmount = ConvertFromWei(response.FromTokenAmount, fromToken);
        var toAmount = ConvertFromWei(response.ToTokenAmount, toToken);

        return new SwapQuote
        {
            FromToken = fromToken,
            ToToken = toToken,
            FromTokenAmount = fromAmount,
            ToTokenAmount = toAmount,
            ExchangeRate = fromAmount > 0 ? toAmount / fromAmount : 0,
            EstimatedGas = response.EstimatedGas.ToString(),
            Provider = PROVIDER_NAME,
            QuotedAt = DateTime.UtcNow
        };
    }

    private DexSwapTransaction MapToSwapTransaction(OneInchSwapResponse response, string fromAddress)
    {
        var fromAmount = ConvertFromWei(response.FromTokenAmount, response.FromToken.Address);
        var toAmount = ConvertFromWei(response.ToTokenAmount, response.ToToken.Address);

        return new DexSwapTransaction
        {
            FromToken = response.FromToken.Address,
            ToToken = response.ToToken.Address,
            FromTokenAmount = response.FromTokenAmount,
            ToTokenAmount = response.ToTokenAmount,
            ExchangeRate = fromAmount > 0 ? toAmount / fromAmount : 0,
            To = response.Tx.To,
            Data = response.Tx.Data,
            Value = response.Tx.Value,
            Gas = response.Tx.Gas.ToString(),
            GasPrice = response.Tx.GasPrice,
            SpenderAddress = response.Tx.To, // Router contract is the spender
            PlatformFee = 0, // Set by fee service later
            MinimumReceived = 0 // Set by slippage service later
        };
    }

    // Mock mode implementations for testing without real API
    private SwapQuote GetMockQuote(string fromToken, string toToken, decimal amount)
    {
        var mockRate = GetMockExchangeRate(fromToken, toToken);
        var toAmount = amount * mockRate;

        _logger.LogInformation("Using MOCK 1inch quote: {Amount} {From} = {ToAmount} {To}",
            amount, TestnetTokens.GetSymbol(fromToken), toAmount, TestnetTokens.GetSymbol(toToken));

        return new SwapQuote
        {
            FromToken = fromToken,
            ToToken = toToken,
            FromTokenAmount = amount,
            ToTokenAmount = toAmount,
            ExchangeRate = mockRate,
            EstimatedGas = "150000",
            Provider = PROVIDER_NAME,
            QuotedAt = DateTime.UtcNow
        };
    }

    private DexSwapTransaction GetMockSwapTransaction(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippageTolerance,
        string fromAddress)
    {
        var mockRate = GetMockExchangeRate(fromToken, toToken);
        var toAmount = amount * mockRate;
        var fromAmountWei = ConvertToWei(amount, fromToken);
        var toAmountWei = ConvertToWei(toAmount, toToken);

        _logger.LogInformation("Using MOCK 1inch swap transaction");

        return new DexSwapTransaction
        {
            FromToken = fromToken,
            ToToken = toToken,
            FromTokenAmount = fromAmountWei,
            ToTokenAmount = toAmountWei,
            ExchangeRate = mockRate,
            To = "0x1111111254fb6c44bAC0beD2854e76F90643097d", // Mock 1inch router
            Data = "0xmockdata",
            Value = "0",
            Gas = "150000",
            GasPrice = "30000000000",
            SpenderAddress = "0x1111111254fb6c44bAC0beD2854e76F90643097d",
            PlatformFee = 0,
            MinimumReceived = 0
        };
    }

    private decimal GetMockExchangeRate(string fromToken, string toToken)
    {
        var fromSymbol = TestnetTokens.GetSymbol(fromToken);
        var toSymbol = TestnetTokens.GetSymbol(toToken);

        // Mock exchange rates
        return (fromSymbol, toSymbol) switch
        {
            ("USDC", "WETH") => 0.000285m, // 1 USDC = 0.000285 WETH (~$3500 ETH)
            ("WETH", "USDC") => 3500m,      // 1 WETH = 3500 USDC
            ("USDC", "WMATIC") => 1.25m,    // 1 USDC = 1.25 WMATIC (~$0.80 MATIC)
            ("WMATIC", "USDC") => 0.80m,    // 1 WMATIC = 0.80 USDC
            ("WETH", "WMATIC") => 4375m,    // 1 WETH = 4375 WMATIC
            ("WMATIC", "WETH") => 0.000228m,// 1 WMATIC = 0.000228 WETH
            _ => 1m // Same token or unknown pair
        };
    }
}
