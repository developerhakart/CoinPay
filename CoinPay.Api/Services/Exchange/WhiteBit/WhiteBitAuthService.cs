using System.Security.Cryptography;
using System.Text;

namespace CoinPay.Api.Services.Exchange.WhiteBit;

/// <summary>
/// WhiteBit authentication service implementation
/// </summary>
public class WhiteBitAuthService : IWhiteBitAuthService
{
    private readonly IWhiteBitApiClient _apiClient;
    private readonly ILogger<WhiteBitAuthService> _logger;

    public WhiteBitAuthService(
        IWhiteBitApiClient apiClient,
        ILogger<WhiteBitAuthService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<bool> ValidateCredentialsAsync(string apiKey, string apiSecret)
    {
        try
        {
            _logger.LogInformation("Validating WhiteBit credentials");
            return await _apiClient.TestConnectionAsync(apiKey, apiSecret);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Credential validation failed");
            return false;
        }
    }

    public string GenerateSignature(string apiSecret, string method, string path, string body, long nonce)
    {
        var message = $"{method}{path}{nonce}{body}";
        var keyBytes = Encoding.UTF8.GetBytes(apiSecret);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(messageBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    public long GenerateNonce()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
