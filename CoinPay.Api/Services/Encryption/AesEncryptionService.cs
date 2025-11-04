using System.Security.Cryptography;
using System.Text;

namespace CoinPay.Api.Services.Encryption;

/// <summary>
/// AES-256-GCM encryption service for sensitive data
/// Uses authenticated encryption for both confidentiality and integrity
/// </summary>
public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly ILogger<AesEncryptionService> _logger;
    private const int NonceSize = 12; // 96 bits (recommended for GCM)
    private const int TagSize = 16;   // 128 bits (authentication tag)

    public AesEncryptionService(IConfiguration configuration, ILogger<AesEncryptionService> logger)
    {
        _logger = logger;

        // Get encryption key from environment variable or configuration
        var keyBase64 = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
                       ?? configuration["Encryption:Key"];

        if (string.IsNullOrEmpty(keyBase64))
        {
            // For development only - generate a temporary key
            // NEVER use this in production!
            _logger.LogWarning("No encryption key found. Generating temporary key for development. " +
                             "Set ENCRYPTION_KEY environment variable in production!");
            _key = GenerateKey();
            _logger.LogWarning("Generated encryption key (Base64): {Key}", Convert.ToBase64String(_key));
        }
        else
        {
            try
            {
                _key = Convert.FromBase64String(keyBase64);
                if (_key.Length != 32)
                {
                    throw new ArgumentException("Encryption key must be 32 bytes (256 bits)");
                }
                _logger.LogInformation("Encryption key loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load encryption key");
                throw new InvalidOperationException("Invalid encryption key configuration", ex);
            }
        }
    }

    public byte[] Encrypt(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
        {
            throw new ArgumentNullException(nameof(plaintext));
        }

        try
        {
            // Generate random nonce (IV)
            var nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce);

            // Convert plaintext to bytes
            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            // Prepare buffer for encrypted data
            var ciphertext = new byte[plaintextBytes.Length];
            var tag = new byte[TagSize];

            // Encrypt using AES-GCM
            using var aesGcm = new AesGcm(_key, TagSize);
            aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

            // Combine: nonce + ciphertext + tag
            var result = new byte[NonceSize + ciphertext.Length + TagSize];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(ciphertext, 0, result, NonceSize, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, result, NonceSize + ciphertext.Length, TagSize);

            _logger.LogDebug("Data encrypted successfully. Size: {Size} bytes", result.Length);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encryption failed");
            throw new InvalidOperationException("Encryption failed", ex);
        }
    }

    public string Decrypt(byte[] encryptedData)
    {
        if (encryptedData == null || encryptedData.Length < NonceSize + TagSize)
        {
            throw new ArgumentException("Invalid encrypted data", nameof(encryptedData));
        }

        try
        {
            // Extract components
            var nonce = new byte[NonceSize];
            var tag = new byte[TagSize];
            var ciphertextLength = encryptedData.Length - NonceSize - TagSize;
            var ciphertext = new byte[ciphertextLength];

            Buffer.BlockCopy(encryptedData, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(encryptedData, NonceSize, ciphertext, 0, ciphertextLength);
            Buffer.BlockCopy(encryptedData, NonceSize + ciphertextLength, tag, 0, TagSize);

            // Decrypt using AES-GCM
            var plaintextBytes = new byte[ciphertextLength];
            using var aesGcm = new AesGcm(_key, TagSize);
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintextBytes);

            var plaintext = Encoding.UTF8.GetString(plaintextBytes);
            _logger.LogDebug("Data decrypted successfully");
            return plaintext;
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex, "Decryption failed - data may be corrupted or tampered with");
            throw new InvalidOperationException("Decryption failed - data integrity check failed", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Decryption failed");
            throw new InvalidOperationException("Decryption failed", ex);
        }
    }

    public bool VerifyEncryption()
    {
        try
        {
            const string testData = "Test encryption verification";
            var encrypted = Encrypt(testData);
            var decrypted = Decrypt(encrypted);
            return testData == decrypted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encryption verification failed");
            return false;
        }
    }

    /// <summary>
    /// Generate a new 256-bit encryption key
    /// Use this once to generate a key, then store it securely
    /// </summary>
    private static byte[] GenerateKey()
    {
        var key = new byte[32]; // 256 bits
        RandomNumberGenerator.Fill(key);
        return key;
    }

    /// <summary>
    /// Static method to generate and display a new encryption key
    /// Run this once during initial setup
    /// </summary>
    public static string GenerateNewKey()
    {
        var key = GenerateKey();
        return Convert.ToBase64String(key);
    }
}
