using Microsoft.Extensions.Options;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace CoinPay.Api.Services.Vault;

/// <summary>
/// Service for interacting with HashiCorp Vault to retrieve secrets
/// </summary>
public class VaultService : IVaultService
{
    private readonly IVaultClient _vaultClient;
    private readonly VaultOptions _options;
    private readonly ILogger<VaultService> _logger;

    public VaultService(IOptions<VaultOptions> options, ILogger<VaultService> logger)
    {
        _options = options.Value;
        _logger = logger;

        // Initialize Vault client
        IAuthMethodInfo authMethod = new TokenAuthMethodInfo(_options.Token);
        var vaultClientSettings = new VaultClientSettings(_options.Address, authMethod)
        {
            VaultServiceTimeout = TimeSpan.FromSeconds(_options.TimeoutSeconds)
        };

        _vaultClient = new VaultClient(vaultClientSettings);

        _logger.LogInformation("VaultService initialized. Address: {Address}, MountPoint: {MountPoint}, BasePath: {BasePath}",
            _options.Address, _options.MountPoint, _options.BasePath);
    }

    /// <summary>
    /// Tests connectivity to Vault server
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            _logger.LogDebug("Testing Vault connection to {Address}", _options.Address);

            var healthStatus = await _vaultClient.V1.System.GetHealthStatusAsync();

            if (healthStatus.Initialized && !healthStatus.Sealed)
            {
                _logger.LogInformation("Vault connection successful. Server version: {Version}", healthStatus.Version);
                return true;
            }

            _logger.LogWarning("Vault is not ready. Initialized: {Initialized}, Sealed: {Sealed}",
                healthStatus.Initialized, healthStatus.Sealed);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to Vault at {Address}", _options.Address);
            return false;
        }
    }

    /// <summary>
    /// Retrieves a secret from Vault with retry logic
    /// </summary>
    public async Task<Dictionary<string, string>> GetSecretAsync(string path)
    {
        var fullPath = $"{_options.BasePath}/{path}";

        for (int attempt = 1; attempt <= _options.RetryAttempts; attempt++)
        {
            try
            {
                _logger.LogDebug("Retrieving secret from path: {Path} (Attempt {Attempt}/{MaxAttempts})",
                    fullPath, attempt, _options.RetryAttempts);

                Secret<SecretData> secret = await _vaultClient.V1.Secrets.KeyValue.V2
                    .ReadSecretAsync(path: fullPath, mountPoint: _options.MountPoint);

                if (secret?.Data?.Data == null)
                {
                    _logger.LogWarning("Secret at path {Path} returned null data", fullPath);
                    return new Dictionary<string, string>();
                }

                // Convert object dictionary to string dictionary
                var result = secret.Data.Data.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToString() ?? string.Empty
                );

                _logger.LogInformation("Successfully retrieved secret from {Path} with {Count} fields",
                    fullPath, result.Count);

                return result;
            }
            catch (Exception ex) when (attempt < _options.RetryAttempts)
            {
                _logger.LogWarning(ex, "Failed to retrieve secret from {Path}. Retrying in {Delay}ms...",
                    fullPath, _options.RetryDelayMs);
                await Task.Delay(_options.RetryDelayMs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve secret from {Path} after {Attempts} attempts",
                    fullPath, _options.RetryAttempts);
                throw new InvalidOperationException(
                    $"Failed to retrieve secret from Vault path '{fullPath}' after {_options.RetryAttempts} attempts", ex);
            }
        }

        return new Dictionary<string, string>();
    }

    /// <summary>
    /// Retrieves a specific field from a secret
    /// </summary>
    public async Task<string?> GetSecretFieldAsync(string path, string field)
    {
        try
        {
            var secret = await GetSecretAsync(path);

            if (secret.TryGetValue(field, out var value))
            {
                _logger.LogDebug("Successfully retrieved field '{Field}' from secret path {Path}", field, path);
                return value;
            }

            _logger.LogWarning("Field '{Field}' not found in secret path {Path}", field, path);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving field '{Field}' from secret path {Path}", field, path);
            throw;
        }
    }

    /// <summary>
    /// Loads all CoinPay secrets from Vault
    /// </summary>
    public async Task<Dictionary<string, Dictionary<string, string>>> LoadAllSecretsAsync()
    {
        _logger.LogInformation("Loading all secrets from Vault...");

        var allSecrets = new Dictionary<string, Dictionary<string, string>>();
        var secretPaths = new[]
        {
            "database",
            "redis",
            "circle",
            "jwt",
            "gateway",
            "blockchain"
        };

        foreach (var secretPath in secretPaths)
        {
            try
            {
                var secret = await GetSecretAsync(secretPath);
                allSecrets[secretPath] = secret;
                _logger.LogInformation("Loaded {Count} fields from secret path: {Path}",
                    secret.Count, secretPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load secret from path: {Path}. Continuing with other secrets...",
                    secretPath);
                // Continue loading other secrets even if one fails
            }
        }

        _logger.LogInformation("Loaded {TotalPaths} secret paths from Vault", allSecrets.Count);
        return allSecrets;
    }
}
