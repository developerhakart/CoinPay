namespace CoinPay.Api.Services.Encryption;

/// <summary>
/// Interface for encryption/decryption of sensitive data
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypt plaintext string to encrypted byte array
    /// </summary>
    /// <param name="plaintext">Plain text to encrypt</param>
    /// <returns>Encrypted data as byte array</returns>
    byte[] Encrypt(string plaintext);

    /// <summary>
    /// Decrypt encrypted byte array to plaintext string
    /// </summary>
    /// <param name="encryptedData">Encrypted data</param>
    /// <returns>Decrypted plaintext</returns>
    string Decrypt(byte[] encryptedData);

    /// <summary>
    /// Verify encryption is working correctly (for health checks)
    /// </summary>
    /// <returns>True if encryption/decryption works</returns>
    bool VerifyEncryption();
}
