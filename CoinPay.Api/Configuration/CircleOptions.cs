namespace CoinPay.Api.Configuration;

/// <summary>
/// Configuration options for Circle Web3 Services SDK
/// </summary>
public class CircleOptions
{
    /// <summary>
    /// Circle API base URL
    /// </summary>
    public string ApiUrl { get; set; } = string.Empty;

    /// <summary>
    /// Circle API key for authentication
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Entity secret for encryption
    /// </summary>
    public string EntitySecret { get; set; } = string.Empty;

    /// <summary>
    /// Circle Application ID
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// Validate configuration
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiUrl))
            throw new InvalidOperationException("Circle ApiUrl is required");

        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new InvalidOperationException("Circle ApiKey is required");

        if (string.IsNullOrWhiteSpace(AppId))
            throw new InvalidOperationException("Circle AppId is required");
    }
}
