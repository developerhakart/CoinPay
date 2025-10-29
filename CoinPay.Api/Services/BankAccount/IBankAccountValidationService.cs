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
}
