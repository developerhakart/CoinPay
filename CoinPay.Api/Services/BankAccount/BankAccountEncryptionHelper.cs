using CoinPay.Api.Services.Encryption;

namespace CoinPay.Api.Services.BankAccount;

/// <summary>
/// Static helper class for encrypting/decrypting bank account sensitive data
/// </summary>
public static class BankAccountEncryptionHelper
{
    /// <summary>
    /// Encrypt routing number
    /// </summary>
    public static byte[] EncryptRoutingNumber(string routingNumber, IEncryptionService encryptionService)
    {
        if (string.IsNullOrWhiteSpace(routingNumber))
        {
            throw new ArgumentException("Routing number cannot be empty", nameof(routingNumber));
        }

        return encryptionService.Encrypt(routingNumber);
    }

    /// <summary>
    /// Decrypt routing number
    /// </summary>
    public static string DecryptRoutingNumber(byte[] encryptedRoutingNumber, IEncryptionService encryptionService)
    {
        if (encryptedRoutingNumber == null || encryptedRoutingNumber.Length == 0)
        {
            throw new ArgumentException("Encrypted routing number cannot be empty", nameof(encryptedRoutingNumber));
        }

        return encryptionService.Decrypt(encryptedRoutingNumber);
    }

    /// <summary>
    /// Encrypt account number
    /// </summary>
    public static byte[] EncryptAccountNumber(string accountNumber, IEncryptionService encryptionService)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException("Account number cannot be empty", nameof(accountNumber));
        }

        return encryptionService.Encrypt(accountNumber);
    }

    /// <summary>
    /// Decrypt account number
    /// </summary>
    public static string DecryptAccountNumber(byte[] encryptedAccountNumber, IEncryptionService encryptionService)
    {
        if (encryptedAccountNumber == null || encryptedAccountNumber.Length == 0)
        {
            throw new ArgumentException("Encrypted account number cannot be empty", nameof(encryptedAccountNumber));
        }

        return encryptionService.Decrypt(encryptedAccountNumber);
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
