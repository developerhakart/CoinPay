namespace CoinPay.Api.Services.Encryption;

/// <summary>
/// Service for encrypting and decrypting exchange API credentials with user-level encryption
/// </summary>
public interface IExchangeCredentialEncryptionService
{
    /// <summary>
    /// Encrypt plaintext using user-specific key
    /// </summary>
    Task<string> EncryptAsync(string plaintext, Guid userId);

    /// <summary>
    /// Decrypt ciphertext using user-specific key
    /// </summary>
    Task<string> DecryptAsync(string ciphertext, Guid userId);

    /// <summary>
    /// Generate encryption key for user
    /// </summary>
    string GenerateUserKey(Guid userId);
}
