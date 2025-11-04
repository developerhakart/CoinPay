using Microsoft.Extensions.Options;

namespace CoinPay.Api.Services.Vault;

/// <summary>
/// Extension methods for loading configuration from HashiCorp Vault
/// </summary>
public static class VaultConfigurationExtensions
{
    /// <summary>
    /// Adds HashiCorp Vault configuration to the application
    /// </summary>
    public static IServiceCollection AddVaultConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind Vault options from configuration
        services.Configure<VaultOptions>(configuration.GetSection("Vault"));

        // Register Vault service
        services.AddSingleton<IVaultService, VaultService>();

        return services;
    }

    /// <summary>
    /// Loads secrets from Vault and adds them to the configuration
    /// </summary>
    public static async Task<WebApplicationBuilder> LoadSecretsFromVaultAsync(this WebApplicationBuilder builder)
    {
        var logger = builder.Logging.Services.BuildServiceProvider()
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("VaultConfiguration");

        logger.LogInformation("Starting Vault configuration loading...");

        try
        {
            // Get Vault options from configuration
            var vaultOptions = new VaultOptions();
            builder.Configuration.GetSection("Vault").Bind(vaultOptions);

            // Create Vault service
            var vaultLogger = builder.Logging.Services.BuildServiceProvider()
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger<VaultService>();

            var vaultService = new VaultService(
                Options.Create(vaultOptions),
                vaultLogger
            );

            // Test Vault connectivity
            logger.LogInformation("Testing Vault connectivity at {Address}...", vaultOptions.Address);
            bool isConnected = await vaultService.TestConnectionAsync();

            if (!isConnected)
            {
                logger.LogError("CRITICAL: Failed to connect to Vault at {Address}. Application will not start without Vault connectivity.",
                    vaultOptions.Address);
                logger.LogError("Please ensure Vault is running: docker-compose up vault");
                logger.LogError("Vault health check endpoint: {Address}/v1/sys/health", vaultOptions.Address);
                throw new InvalidOperationException($"Cannot connect to Vault at {vaultOptions.Address}. Application requires Vault for configuration.");
            }

            logger.LogInformation("Vault connectivity test passed. Loading secrets...");

            // Load all secrets from Vault
            var allSecrets = await vaultService.LoadAllSecretsAsync();

            if (allSecrets.Count == 0)
            {
                logger.LogWarning("WARNING: No secrets loaded from Vault. This may cause application errors.");
            }

            // Map secrets to configuration structure
            var configDictionary = new Dictionary<string, string>();

            // Database secrets
            if (allSecrets.TryGetValue("database", out var dbSecrets))
            {
                MapSecret(configDictionary, dbSecrets, "ConnectionStrings:DefaultConnection", "connection_string");
                logger.LogInformation("Database configuration loaded from Vault");
            }

            // Redis secrets
            if (allSecrets.TryGetValue("redis", out var redisSecrets))
            {
                MapSecret(configDictionary, redisSecrets, "ConnectionStrings:Redis", "connection_string");
                logger.LogInformation("Redis configuration loaded from Vault");
            }

            // Circle API secrets
            if (allSecrets.TryGetValue("circle", out var circleSecrets))
            {
                MapSecret(configDictionary, circleSecrets, "Circle:ApiKey", "api_key");
                MapSecret(configDictionary, circleSecrets, "Circle:EntitySecret", "entity_secret");
                MapSecret(configDictionary, circleSecrets, "Circle:WebhookSecret", "webhook_secret");
                MapSecret(configDictionary, circleSecrets, "Circle:ApiBaseUrl", "api_base_url");
                MapSecret(configDictionary, circleSecrets, "Circle:AppId", "app_id");
                logger.LogInformation("Circle API configuration loaded from Vault");
            }

            // JWT secrets
            if (allSecrets.TryGetValue("jwt", out var jwtSecrets))
            {
                MapSecret(configDictionary, jwtSecrets, "Jwt:SecretKey", "secret_key");
                MapSecret(configDictionary, jwtSecrets, "Jwt:Issuer", "issuer");
                MapSecret(configDictionary, jwtSecrets, "Jwt:Audience", "audience");
                MapSecret(configDictionary, jwtSecrets, "Jwt:ExpirationMinutes", "expiration_minutes");
                MapSecret(configDictionary, jwtSecrets, "Jwt:RefreshTokenExpirationDays", "refresh_token_expiration_days");
                logger.LogInformation("JWT configuration loaded from Vault");
            }

            // Gateway secrets
            if (allSecrets.TryGetValue("gateway", out var gatewaySecrets))
            {
                MapSecret(configDictionary, gatewaySecrets, "Gateway:WebhookSecret", "webhook_secret");
                logger.LogInformation("Gateway configuration loaded from Vault");
            }

            // Blockchain secrets
            if (allSecrets.TryGetValue("blockchain", out var blockchainSecrets))
            {
                MapSecret(configDictionary, blockchainSecrets, "Blockchain:TestWallet:PrivateKey", "test_wallet_private_key");
                logger.LogInformation("Blockchain configuration loaded from Vault");
            }

            // Add Vault secrets to configuration
            builder.Configuration.AddInMemoryCollection(configDictionary!);

            logger.LogInformation("Successfully loaded {Count} configuration values from Vault", configDictionary.Count);

            // Log configuration keys (but not values!) for debugging
            logger.LogDebug("Vault configuration keys loaded: {Keys}",
                string.Join(", ", configDictionary.Keys));
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "FATAL ERROR: Failed to load configuration from Vault. Application cannot start.");
            logger.LogError("Vault Address: {Address}", builder.Configuration["Vault:Address"]);
            logger.LogError("Check the following:");
            logger.LogError("1. Vault container is running: docker-compose ps");
            logger.LogError("2. Vault is healthy: curl {Address}/v1/sys/health", builder.Configuration["Vault:Address"]);
            logger.LogError("3. Secrets are initialized: ./vault/scripts/init-secrets.ps1");
            throw;
        }

        return builder;
    }

    private static void MapSecret(
        Dictionary<string, string> configDictionary,
        Dictionary<string, string> secrets,
        string configKey,
        string secretKey)
    {
        if (secrets.TryGetValue(secretKey, out var value))
        {
            configDictionary[configKey] = value;
        }
    }
}
