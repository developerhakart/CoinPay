using System.ComponentModel.DataAnnotations;

namespace CoinPay.Api.DTOs;

/// <summary>
/// Request DTO for initiating a USDC transfer
/// </summary>
public class TransferRequest
{
    /// <summary>
    /// Recipient wallet address
    /// </summary>
    [Required]
    [RegularExpression(@"^0x[a-fA-F0-9]{40}$", ErrorMessage = "Invalid Ethereum address format")]
    public string ToAddress { get; set; } = string.Empty;

    /// <summary>
    /// Transfer amount in USDC
    /// </summary>
    [Required]
    [Range(0.000001, 1000000, ErrorMessage = "Amount must be between 0.000001 and 1,000,000 USDC")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Token contract address (defaults to USDC on Polygon Amoy)
    /// </summary>
    public string? TokenAddress { get; set; }

    /// <summary>
    /// Optional description/note for the transfer
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}

/// <summary>
/// Response DTO for transfer submission
/// </summary>
public class TransferResponse
{
    public int TransactionId { get; set; }
    public string UserOpHash { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TokenAddress { get; set; } = string.Empty;
    public bool IsGasless { get; set; } = true;
    public DateTime SubmittedAt { get; set; }
}

/// <summary>
/// Response DTO for transaction status query
/// </summary>
public class TransactionStatusResponse
{
    public int TransactionId { get; set; }
    public string UserOpHash { get; set; } = string.Empty;
    public string? TransactionHash { get; set; }
    public string Status { get; set; } = "Pending";
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TokenAddress { get; set; } = string.Empty;
    public bool IsGasless { get; set; }
    public decimal GasUsed { get; set; }
    public long? BlockNumber { get; set; }
    public int Confirmations { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ExplorerUrl { get; set; }
    public string? UserOpExplorerUrl { get; set; }
}

/// <summary>
/// Response DTO for transaction history
/// </summary>
public class TransactionHistoryResponse
{
    public List<TransactionStatusResponse> Transactions { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Response DTO for detailed transaction information
/// </summary>
public class TransactionDetailResponse
{
    public int Id { get; set; }
    public string UserOpHash { get; set; } = string.Empty;
    public string? TransactionHash { get; set; }

    // Addresses
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;

    // Amount
    public string Amount { get; set; } = string.Empty;
    public decimal AmountDecimal { get; set; }
    public string FormattedAmount { get; set; } = string.Empty;
    public string TokenAddress { get; set; } = string.Empty;
    public string TokenSymbol { get; set; } = "USDC";

    // Status
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public string? ErrorMessage { get; set; }

    // Blockchain info
    public int ChainId { get; set; }
    public string ChainName { get; set; } = string.Empty;
    public long? BlockNumber { get; set; }
    public int Confirmations { get; set; }
    public decimal GasUsed { get; set; }
    public decimal GasPaidByUser { get; set; } = 0; // Gasless transactions
    public bool IsGasless { get; set; }
    public string? ExplorerUrl { get; set; }
    public string? UserOpExplorerUrl { get; set; }

    // User Operation details
    public string? Nonce { get; set; }
    public string? Signature { get; set; }
    public string TransactionType { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for wallet balance
/// </summary>
public class BalanceResponse
{
    public string Address { get; set; } = string.Empty;
    public string TokenAddress { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "USDC";
    public DateTime LastUpdated { get; set; }
}
