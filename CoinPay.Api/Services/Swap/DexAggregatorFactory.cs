using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Factory for creating DEX aggregator service instances
/// </summary>
public class DexAggregatorFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DexAggregatorFactory> _logger;

    public DexAggregatorFactory(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<DexAggregatorFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Gets a DEX aggregator service provider by name
    /// </summary>
    /// <param name="providerName">Provider name (1inch, 0x, etc.)</param>
    /// <returns>DEX aggregator service instance</returns>
    /// <exception cref="ArgumentException">Thrown when provider is unknown</exception>
    public IDexAggregatorService GetProvider(string providerName)
    {
        var normalizedProvider = providerName.ToLower();

        _logger.LogInformation("Creating DEX aggregator provider: {Provider}", normalizedProvider);

        return normalizedProvider switch
        {
            "1inch" => (IDexAggregatorService)_serviceProvider.GetRequiredService(typeof(OneInchAggregatorService)),
            "0x" => throw new NotImplementedException("0x aggregator not yet implemented"),
            _ => throw new ArgumentException($"Unknown DEX aggregator provider: {providerName}")
        };
    }

    /// <summary>
    /// Gets the default DEX aggregator service (configured in appsettings)
    /// </summary>
    /// <returns>Default DEX aggregator service instance</returns>
    public IDexAggregatorService GetDefaultProvider()
    {
        var defaultProvider = _configuration["Swap:DefaultProvider"] ?? "1inch";
        return GetProvider(defaultProvider);
    }
}
