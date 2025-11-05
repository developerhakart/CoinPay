namespace CoinPay.Api.Services.Exchange.WhiteBit;

/// <summary>
/// Service for WhiteBit authentication operations
/// </summary>
public interface IWhiteBitAuthService
{
    /// <summary>
    /// Validate API credentials by testing connection
    /// </summary>
    Task<bool> ValidateCredentialsAsync(string apiKey, string apiSecret);

    /// <summary>
    /// Generate HMAC-SHA256 signature for request
    /// </summary>
    string GenerateSignature(string apiSecret, string method, string path, string body, long nonce);

    /// <summary>
    /// Generate nonce (timestamp in milliseconds)
    /// </summary>
    long GenerateNonce();
}
