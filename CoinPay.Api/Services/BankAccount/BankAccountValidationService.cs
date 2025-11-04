using CoinPay.Api.Repositories;
using CoinPay.Api.Services.Encryption;

namespace CoinPay.Api.Services.BankAccount;

/// <summary>
/// Service for validating bank account information
/// Implements US ACH validation rules with duplicate detection
/// </summary>
public class BankAccountValidationService : IBankAccountValidationService
{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IEncryptionService _encryptionService;

    // Common US bank routing numbers for validation and lookup
    private static readonly Dictionary<string, string> KnownRoutingNumbers = new()
    {
        { "011401533", "Wells Fargo" },
        { "021000021", "JPMorgan Chase" },
        { "026009593", "Bank of America" },
        { "121000248", "Wells Fargo" },
        { "122000247", "U.S. Bank" },
        { "061000104", "SunTrust Bank" },
        { "053101121", "BB&T" },
        { "063100277", "Fifth Third Bank" },
        { "091000019", "Bank of the West" },
        { "111000025", "Chase Bank" },
        { "021001088", "Citibank" },
        { "031201360", "Ally Bank" },
        { "041000014", "PNC Bank" },
        { "051000017", "BMO Harris Bank" },
        { "071000013", "Regions Bank" },
        { "081000210", "KeyBank" },
        { "101000187", "TD Bank" },
        { "267084131", "USAA Federal Savings Bank" },
    };

    public BankAccountValidationService(
        IBankAccountRepository bankAccountRepository,
        IEncryptionService encryptionService)
    {
        _bankAccountRepository = bankAccountRepository;
        _encryptionService = encryptionService;
    }
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

    /// <summary>
    /// Check if bank account already exists for user (duplicate detection)
    /// </summary>
    public async Task<bool> IsDuplicateAccountAsync(int userId, string routingNumber, string accountNumber, Guid? excludeAccountId = null)
    {
        // Get all user's bank accounts
        var userAccounts = await _bankAccountRepository.GetAllByUserIdAsync(userId);

        // Filter out excluded account if provided
        if (excludeAccountId.HasValue)
        {
            userAccounts = userAccounts.Where(a => a.Id != excludeAccountId.Value);
        }

        // Clean input for comparison
        var cleanRoutingNumber = new string(routingNumber.Where(char.IsDigit).ToArray());
        var cleanAccountNumber = new string(accountNumber.Where(char.IsDigit).ToArray());

        // Check each account for duplicate
        foreach (var account in userAccounts)
        {
            // Decrypt stored values for comparison
            var storedRoutingNumber = BankAccountEncryptionHelper.DecryptRoutingNumber(
                account.RoutingNumberEncrypted, _encryptionService);
            var storedAccountNumber = BankAccountEncryptionHelper.DecryptAccountNumber(
                account.AccountNumberEncrypted, _encryptionService);

            // Clean stored values
            var cleanStoredRouting = new string(storedRoutingNumber.Where(char.IsDigit).ToArray());
            var cleanStoredAccount = new string(storedAccountNumber.Where(char.IsDigit).ToArray());

            // Check for exact match
            if (cleanRoutingNumber == cleanStoredRouting && cleanAccountNumber == cleanStoredAccount)
            {
                return true; // Duplicate found
            }
        }

        return false; // No duplicate
    }

    /// <summary>
    /// Lookup bank name from routing number
    /// </summary>
    public string? LookupBankName(string routingNumber)
    {
        var digitsOnly = new string(routingNumber.Where(char.IsDigit).ToArray());

        if (KnownRoutingNumbers.TryGetValue(digitsOnly, out var bankName))
        {
            return bankName;
        }

        return null;
    }

    /// <summary>
    /// Validate complete bank account with detailed result
    /// </summary>
    public ValidationResult ValidateComplete(BankAccountValidationRequest request)
    {
        var result = new ValidationResult { IsValid = true };

        // Validate account holder name
        var nameValidation = ValidateAccountHolderName(request.AccountHolderName);
        if (!nameValidation.IsValid)
        {
            result.IsValid = false;
            result.AddError(nameValidation.ErrorMessage!);
        }

        // Validate routing number
        var routingValidation = ValidateRoutingNumber(request.RoutingNumber);
        if (!routingValidation.IsValid)
        {
            result.IsValid = false;
            result.AddError(routingValidation.ErrorMessage!);
        }
        else
        {
            // Lookup bank name from routing number
            var suggestedBankName = LookupBankName(request.RoutingNumber);
            if (suggestedBankName != null)
            {
                result.SuggestedBankName = suggestedBankName;

                // Warn if provided bank name doesn't match
                if (!string.IsNullOrEmpty(request.BankName) &&
                    !request.BankName.Equals(suggestedBankName, StringComparison.OrdinalIgnoreCase))
                {
                    result.AddWarning($"Bank name '{request.BankName}' doesn't match routing number. Expected: '{suggestedBankName}'");
                }
            }
        }

        // Validate account number
        var accountValidation = ValidateAccountNumber(request.AccountNumber);
        if (!accountValidation.IsValid)
        {
            result.IsValid = false;
            result.AddError(accountValidation.ErrorMessage!);
        }

        // Validate account type
        if (string.IsNullOrWhiteSpace(request.AccountType))
        {
            result.IsValid = false;
            result.AddError("Account type is required");
        }
        else if (request.AccountType.ToLowerInvariant() != "checking" &&
                 request.AccountType.ToLowerInvariant() != "savings")
        {
            result.IsValid = false;
            result.AddError("Account type must be 'checking' or 'savings'");
        }

        return result;
    }
}
