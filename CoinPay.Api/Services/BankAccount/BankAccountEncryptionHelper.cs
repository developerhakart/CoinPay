using CoinPay.Api.Services.Encryption;

namespace CoinPay.Api.Services.BankAccount;

/// <summary>
/// Helper class for encrypting/decrypting bank account sensitive data
/// </summary>
public class BankAccountEncryptionHelper
{
    private readonly IEncryptionService _encryptionService;

    public BankAccountEncryptionHelper(IEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    /// <summary>
    /// Encrypt routing number
    /// </summary>
    public byte[] EncryptRoutingNumber(string routingNumber)
    {
        if (string.IsNullOrWhiteSpace(routingNumber))
        {
            throw new ArgumentException("Routing number cannot be empty", nameof(routingNumber));
        }

        return _encryptionService.Encrypt(routingNumber);
    }

    /// <summary>
    /// Decrypt routing number
    /// </summary>
    public string DecryptRoutingNumber(byte[] encryptedRoutingNumber)
    {
        if (encryptedRoutingNumber == null || encryptedRoutingNumber.Length == 0)
        {
            throw new ArgumentException("Encrypted routing number cannot be empty", nameof(encryptedRoutingNumber));
        }

        return _encryptionService.Decrypt(encryptedRoutingNumber);
    }

    /// <summary>
    /// Encrypt account number
    /// </summary>
    public byte[] EncryptAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException("Account number cannot be empty", nameof(accountNumber));
        }

        return _encryptionService.Encrypt(accountNumber);
    }

    /// <summary>
    /// Decrypt account number
    /// </summary>
    public string DecryptAccountNumber(byte[] encryptedAccountNumber)
    {
        if (encryptedAccountNumber == null || encryptedAccountNumber.Length == 0)
        {
            throw new ArgumentException("Encrypted account number cannot be empty", nameof(encryptedAccountNumber));
        }

        return _encryptionService.Decrypt(encryptedAccountNumber);
    }

    /// <summary>
    /// Get last 4 digits from account number for display
    /// </summary>
    public static string GetLastFourDigits(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            return string.Empty;
        }

        // Remove any non-digit characters
        var digitsOnly = new string(accountNumber.Where(char.IsDigit).ToArray());

        if (digitsOnly.Length <= 4)
        {
            return digitsOnly;
        }

        return digitsOnly.Substring(digitsOnly.Length - 4);
    }
}
