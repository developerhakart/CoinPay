namespace CoinPay.Api.Services.BankAccount;

/// <summary>
/// Interface for bank account validation service
/// </summary>
public interface IBankAccountValidationService
{
    /// <summary>
    /// Validate US routing number using ABA checksum algorithm
    /// </summary>
    /// <param name="routingNumber">9-digit routing number</param>
    /// <returns>Tuple with validation result and error message if invalid</returns>
    (bool IsValid, string? ErrorMessage) ValidateRoutingNumber(string routingNumber);

    /// <summary>
    /// Validate US bank account number
    /// </summary>
    /// <param name="accountNumber">5-17 digit account number</param>
    /// <returns>Tuple with validation result and error message if invalid</returns>
    (bool IsValid, string? ErrorMessage) ValidateAccountNumber(string accountNumber);

    /// <summary>
    /// Validate account holder name
    /// </summary>
    /// <param name="name">Account holder name</param>
    /// <returns>Tuple with validation result and error message if invalid</returns>
    (bool IsValid, string? ErrorMessage) ValidateAccountHolderName(string name);

    /// <summary>
    /// Extract last 4 digits from account number for display
    /// </summary>
    /// <param name="accountNumber">Full account number</param>
    /// <returns>Last 4 digits</returns>
    string GetLastFourDigits(string accountNumber);

    /// <summary>
    /// Check if bank account already exists for user (duplicate detection)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="routingNumber">Routing number</param>
    /// <param name="accountNumber">Account number</param>
    /// <param name="excludeAccountId">Optional account ID to exclude from check (for updates)</param>
    /// <returns>True if duplicate exists</returns>
    Task<bool> IsDuplicateAccountAsync(int userId, string routingNumber, string accountNumber, Guid? excludeAccountId = null);

    /// <summary>
    /// Lookup bank name from routing number
    /// </summary>
    /// <param name="routingNumber">9-digit routing number</param>
    /// <returns>Bank name if found, null otherwise</returns>
    string? LookupBankName(string routingNumber);

    /// <summary>
    /// Validate complete bank account with detailed result
    /// </summary>
    /// <param name="request">Validation request</param>
    /// <returns>Detailed validation result</returns>
    ValidationResult ValidateComplete(BankAccountValidationRequest request);
}

/// <summary>
/// Request for complete bank account validation
/// </summary>
public class BankAccountValidationRequest
{
    public string AccountHolderName { get; set; } = string.Empty;
    public string RoutingNumber { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string? BankName { get; set; }
}

/// <summary>
/// Detailed validation result
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public string? SuggestedBankName { get; set; }

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);
}
