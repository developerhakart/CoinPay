namespace CoinPay.Api.Services.Vault;

/// <summary>
/// Service for interacting with HashiCorp Vault to retrieve secrets
/// </summary>
public interface IVaultService
{
    /// <summary>
    /// Tests connectivity to Vault server
    /// </summary>
    /// <returns>True if Vault is accessible, false otherwise</returns>
    Task<bool> TestConnectionAsync();

    /// <summary>
    /// Retrieves a secret from Vault
    /// </summary>
    /// <param name="path">Secret path (relative to base path)</param>
    /// <returns>Dictionary of secret key-value pairs</returns>
    Task<Dictionary<string, string>> GetSecretAsync(string path);

    /// <summary>
    /// Retrieves a specific field from a secret
    /// </summary>
    /// <param name="path">Secret path (relative to base path)</param>
    /// <param name="field">Field name</param>
    /// <returns>Field value or null if not found</returns>
    Task<string?> GetSecretFieldAsync(string path, string field);

    /// <summary>
    /// Loads all CoinPay secrets from Vault
    /// </summary>
    /// <returns>Dictionary with all secrets organized by path</returns>
    Task<Dictionary<string, Dictionary<string, string>>> LoadAllSecretsAsync();
}
