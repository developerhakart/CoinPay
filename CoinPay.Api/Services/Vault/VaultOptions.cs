namespace CoinPay.Api.Services.Vault;

/// <summary>
/// Configuration options for HashiCorp Vault
/// </summary>
public class VaultOptions
{
    /// <summary>
    /// Vault server address (e.g., http://localhost:8200)
    /// </summary>
    public string Address { get; set; } = "http://localhost:8200";

    /// <summary>
    /// Vault authentication token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Mount point for KV secrets engine (default: secret)
    /// </summary>
    public string MountPoint { get; set; } = "secret";

    /// <summary>
    /// Base path for CoinPay secrets (default: coinpay)
    /// </summary>
    public string BasePath { get; set; } = "coinpay";

    /// <summary>
    /// Connection timeout in seconds (default: 30)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Number of retry attempts for failed requests (default: 3)
    /// </summary>
    public int RetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts in milliseconds (default: 1000)
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;
}
