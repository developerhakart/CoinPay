using System.Security.Cryptography;
using System.Text;

namespace CoinPay.Api.Services.Encryption;

/// <summary>
/// AES-256-GCM encryption service with user-level keys
/// </summary>
public class ExchangeCredentialEncryptionService : IExchangeCredentialEncryptionService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExchangeCredentialEncryptionService> _logger;
    private readonly string _masterKey;

    public ExchangeCredentialEncryptionService(
        IConfiguration configuration,
        ILogger<ExchangeCredentialEncryptionService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _masterKey = configuration["ExchangeCredentialEncryption:MasterKey"]
            ?? throw new InvalidOperationException("ExchangeCredentialEncryption:MasterKey not configured");
    }

    public async Task<string> EncryptAsync(string plaintext, Guid userId)
    {
        try
        {
            if (string.IsNullOrEmpty(plaintext))
                throw new ArgumentException("Plaintext cannot be null or empty", nameof(plaintext));

            var userKey = GenerateUserKey(userId);
            var keyBytes = Convert.FromBase64String(userKey);

            using var aes = new AesGcm(keyBytes);

            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            var nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // 12 bytes
            var ciphertext = new byte[plaintextBytes.Length];
            var tag = new byte[AesGcm.TagByteSizes.MaxSize]; // 16 bytes

            RandomNumberGenerator.Fill(nonce);

            aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

            // Combine: nonce + tag + ciphertext
            var combined = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, combined, nonce.Length + tag.Length, ciphertext.Length);

            return Convert.ToBase64String(combined);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encryption failed for user {UserId}", userId);
            throw;
        }
    }

    public async Task<string> DecryptAsync(string ciphertext, Guid userId)
    {
        try
        {
            if (string.IsNullOrEmpty(ciphertext))
                throw new ArgumentException("Ciphertext cannot be null or empty", nameof(ciphertext));

            var userKey = GenerateUserKey(userId);
            var keyBytes = Convert.FromBase64String(userKey);

            var combined = Convert.FromBase64String(ciphertext);

            // Extract: nonce + tag + ciphertext
            var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
            var tag = new byte[AesGcm.TagByteSizes.MaxSize];
            var encrypted = new byte[combined.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(combined, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(combined, nonce.Length, tag, 0, tag.Length);
            Buffer.BlockCopy(combined, nonce.Length + tag.Length, encrypted, 0, encrypted.Length);

            using var aes = new AesGcm(keyBytes);
            var plaintext = new byte[encrypted.Length];

            aes.Decrypt(nonce, encrypted, tag, plaintext);

            return Encoding.UTF8.GetString(plaintext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Decryption failed for user {UserId}", userId);
            throw;
        }
    }

    public string GenerateUserKey(Guid userId)
    {
        // Derive user-specific key from master key + user ID
        // In production, consider using PBKDF2 or a key derivation function
        var input = $"{_masterKey}:{userId}";

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

        return Convert.ToBase64String(hash); // 32 bytes = 256 bits
    }
}
