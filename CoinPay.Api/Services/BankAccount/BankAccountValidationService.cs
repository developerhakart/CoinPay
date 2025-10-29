namespace CoinPay.Api.Services.BankAccount;

/// <summary>
/// Service for validating bank account information
/// Implements US ACH validation rules
/// </summary>
public class BankAccountValidationService : IBankAccountValidationService
{
    /// <summary>
    /// Validate US routing number using ABA checksum algorithm
    /// </summary>
    public (bool IsValid, string? ErrorMessage) ValidateRoutingNumber(string routingNumber)
    {
        // Remove any non-digit characters
        var digitsOnly = new string(routingNumber.Where(char.IsDigit).ToArray());

        // Check length
        if (digitsOnly.Length != 9)
        {
            return (false, "Routing number must be exactly 9 digits");
        }

        // Validate checksum using ABA algorithm
        // Multiply digits by weights [3,7,1,3,7,1,3,7,1] and sum
        int[] weights = { 3, 7, 1, 3, 7, 1, 3, 7, 1 };
        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            sum += (digitsOnly[i] - '0') * weights[i];
        }

        // Sum must be divisible by 10
        if (sum % 10 != 0)
        {
            return (false, "Invalid routing number checksum");
        }

        return (true, null);
    }

    /// <summary>
    /// Validate US bank account number
    /// </summary>
    public (bool IsValid, string? ErrorMessage) ValidateAccountNumber(string accountNumber)
    {
        // Remove any non-digit characters
        var digitsOnly = new string(accountNumber.Where(char.IsDigit).ToArray());

        // Check length: 5-17 digits for US accounts
        if (digitsOnly.Length < 5)
        {
            return (false, "Account number must be at least 5 digits");
        }

        if (digitsOnly.Length > 17)
        {
            return (false, "Account number must be at most 17 digits");
        }

        return (true, null);
    }

    /// <summary>
    /// Validate account holder name
    /// </summary>
    public (bool IsValid, string? ErrorMessage) ValidateAccountHolderName(string name)
    {
        var trimmed = name.Trim();

        // Check length
        if (trimmed.Length < 2)
        {
            return (false, "Account holder name must be at least 2 characters");
        }

        if (trimmed.Length > 255)
        {
            return (false, "Account holder name must be at most 255 characters");
        }

        // Check format: letters, spaces, hyphens, apostrophes only
        if (!System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^[a-zA-Z\s\-']+$"))
        {
            return (false, "Account holder name can only contain letters, spaces, hyphens, and apostrophes");
        }

        return (true, null);
    }

    /// <summary>
    /// Extract last 4 digits from account number for display
    /// </summary>
    public string GetLastFourDigits(string accountNumber)
    {
        var digitsOnly = new string(accountNumber.Where(char.IsDigit).ToArray());

        if (digitsOnly.Length <= 4)
        {
            return digitsOnly;
        }

        return digitsOnly[^4..];
    }
}
