using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RestSharp;

namespace CoinPay.Api.Services.Circle;

/// <summary>
/// Service for encrypting entity secrets for Circle API developer-controlled transfers.
/// Uses RSA encryption with OAEP padding and Circle's public key.
/// </summary>
public interface IEntitySecretEncryptionService
{
    /// <summary>
    /// Encrypts the raw entity secret into the ciphertext format required by Circle API
    /// </summary>
    /// <param name="entitySecret">Raw entity secret (hex string)</param>
    /// <returns>Base64-encoded encrypted ciphertext</returns>
    string EncryptEntitySecret(string entitySecret);
}

public class EntitySecretEncryptionService : IEntitySecretEncryptionService
{
    private readonly ILogger<EntitySecretEncryptionService> _logger;
    private readonly CircleOptions _options;
    private RSA? _rsa;
    private readonly SemaphoreSlim _keyLoadSemaphore = new(1, 1);
    private bool _keyLoadAttempted = false;

    public EntitySecretEncryptionService(
        ILogger<EntitySecretEncryptionService> logger,
        IOptions<CircleOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    private async Task<RSA?> GetPublicKeyAsync()
    {
        // If we already have the key or failed to load it, return immediately
        if (_keyLoadAttempted)
        {
            return _rsa;
        }

        await _keyLoadSemaphore.WaitAsync();
        try
        {
            // Double-check after acquiring the lock
            if (_keyLoadAttempted)
            {
                return _rsa;
            }

            _keyLoadAttempted = true;

            // Attempt to fetch public key from Circle API
            try
            {
                _logger.LogInformation("Fetching Circle public key from API...");

                var client = new RestClient(_options.ApiUrl);
                var request = new RestRequest("/config/entity/publicKey", Method.Get);
                request.AddHeader("Authorization", $"Bearer {_options.ApiKey}");

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var json = JsonDocument.Parse(response.Content);
                    if (json.RootElement.TryGetProperty("data", out var data) &&
                        data.TryGetProperty("publicKey", out var publicKeyElement))
                    {
                        var publicKeyPem = publicKeyElement.GetString();
                        if (!string.IsNullOrEmpty(publicKeyPem))
                        {
                            // Import the PEM public key
                            _rsa = RSA.Create();
                            _rsa.ImportFromPem(publicKeyPem);

                            _logger.LogInformation("Circle public key loaded successfully from API");
                            return _rsa;
                        }
                    }
                }

                _logger.LogWarning("Failed to fetch public key from Circle API. Status: {Status}, Content: {Content}",
                    response.StatusCode, response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Circle public key from API");
            }

            // If API fetch failed, return null (entity secret will be used as-is)
            _logger.LogWarning("Public key not available. Entity secret will be sent as-is (may work if already encrypted)");
            return null;
        }
        finally
        {
            _keyLoadSemaphore.Release();
        }
    }

    public string EncryptEntitySecret(string entitySecret)
    {
        // Try to get public key (cached after first call)
        var rsa = GetPublicKeyAsync().GetAwaiter().GetResult();

        if (rsa == null)
        {
            _logger.LogDebug("Public key not available. Returning entity secret as-is");
            return entitySecret;
        }

        try
        {
            // Convert hex entity secret to bytes (entity secret is 64 hex characters = 32 bytes)
            // This matches the Node.js approach: Buffer.from(entitySecretHex, 'hex')
            byte[] dataToEncrypt = Convert.FromHexString(entitySecret);

            // Encrypt with RSA-OAEP-SHA256 padding (standard for Circle API)
            byte[] encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);

            // Convert to Base64 for transmission
            string ciphertext = Convert.ToBase64String(encryptedData);

            _logger.LogDebug("Entity secret encrypted successfully. Ciphertext length: {Length}", ciphertext.Length);

            return ciphertext;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to encrypt entity secret");
            // Return as-is if encryption fails
            _logger.LogWarning("Returning entity secret as-is due to encryption failure");
            return entitySecret;
        }
    }
}
