using System.ComponentModel.DataAnnotations;

namespace CoinPay.Api.DTOs;

/// <summary>
/// Request DTO for adding a new bank account
/// </summary>
public class AddBankAccountRequest
{
    [Required(ErrorMessage = "Account holder name is required")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Account holder name must be between 2 and 255 characters")]
    [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "Account holder name can only contain letters, spaces, hyphens, and apostrophes")]
    public string AccountHolderName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Routing number is required")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "Routing number must be exactly 9 digits")]
    public string RoutingNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Account number is required")]
    [RegularExpression(@"^\d{5,17}$", ErrorMessage = "Account number must be between 5 and 17 digits")]
    public string AccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Account type is required")]
    [RegularExpression(@"^(checking|savings)$", ErrorMessage = "Account type must be 'checking' or 'savings'")]
    public string AccountType { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Bank name must not exceed 100 characters")]
    public string? BankName { get; set; }

    public bool IsPrimary { get; set; }
}

/// <summary>
/// Request DTO for updating an existing bank account
/// Only allows updating metadata (not routing/account numbers for security)
/// </summary>
public class UpdateBankAccountRequest
{
    [Required(ErrorMessage = "Account holder name is required")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Account holder name must be between 2 and 255 characters")]
    [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "Account holder name can only contain letters, spaces, hyphens, and apostrophes")]
    public string AccountHolderName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Bank name must not exceed 100 characters")]
    public string? BankName { get; set; }

    public bool IsPrimary { get; set; }
}

/// <summary>
/// Response DTO for bank account (never includes full routing/account numbers)
/// </summary>
public class BankAccountResponse
{
    public Guid Id { get; set; }
    public string AccountHolderName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string? BankName { get; set; }

    /// <summary>
    /// Only last 4 digits shown for security
    /// </summary>
    public string LastFourDigits { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Response DTO for list of bank accounts
/// </summary>
public class BankAccountListResponse
{
    public List<BankAccountResponse> BankAccounts { get; set; } = new();
    public int Total { get; set; }
}

/// <summary>
/// Response DTO for bank account validation
/// </summary>
public class BankValidationResponse
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public string? SuggestedBankName { get; set; }
}

/// <summary>
/// Response DTO for bank name lookup
/// </summary>
public class BankLookupResponse
{
    public string RoutingNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
}
