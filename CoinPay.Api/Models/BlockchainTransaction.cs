namespace CoinPay.Api.Models;

/// <summary>
/// Represents a blockchain transaction (UserOperation) in the CoinPay system
/// Based on ERC-4337 Account Abstraction standard
/// </summary>
public class BlockchainTransaction
{
    /// <summary>
    /// Internal database ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's wallet ID (foreign key)
    /// </summary>
    public int WalletId { get; set; }

    /// <summary>
    /// Unique UserOperation hash from bundler
    /// </summary>
    public string UserOpHash { get; set; } = string.Empty;

    /// <summary>
    /// On-chain transaction hash (populated after confirmation)
    /// </summary>
    public string? TransactionHash { get; set; }

    /// <summary>
    /// Sender wallet address
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// Receiver wallet address
    /// </summary>
    public string ToAddress { get; set; } = string.Empty;

    /// <summary>
    /// Token contract address (e.g., USDC)
    /// </summary>
    public string TokenAddress { get; set; } = string.Empty;

    /// <summary>
    /// Transfer amount (stored as string to preserve precision)
    /// </summary>
    public string Amount { get; set; } = string.Empty;

    /// <summary>
    /// Amount in decimal format for display
    /// </summary>
    public decimal AmountDecimal { get; set; }

    /// <summary>
    /// Transaction status: Pending, Confirmed, Failed
    /// </summary>
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    /// <summary>
    /// Chain ID (80002 for Polygon Amoy testnet)
    /// </summary>
    public int ChainId { get; set; }

    /// <summary>
    /// Transaction type (Transfer, Swap, etc.)
    /// </summary>
    public string TransactionType { get; set; } = "Transfer";

    /// <summary>
    /// Gas used (0 for gasless transactions)
    /// </summary>
    public decimal GasUsed { get; set; }

    /// <summary>
    /// Whether gas was sponsored by paymaster
    /// </summary>
    public bool IsGasless { get; set; } = true;

    /// <summary>
    /// Error message if transaction failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Block number where transaction was included
    /// </summary>
    public long? BlockNumber { get; set; }

    /// <summary>
    /// Number of confirmations
    /// </summary>
    public int Confirmations { get; set; }

    /// <summary>
    /// Transaction creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Transaction submitted to bundler timestamp
    /// </summary>
    public DateTime? SubmittedAt { get; set; }

    /// <summary>
    /// Transaction confirmation timestamp
    /// </summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>
    /// Navigation property to Wallet
    /// </summary>
    public Wallet? Wallet { get; set; }
}

/// <summary>
/// Transaction status enumeration
/// </summary>
public enum TransactionStatus
{
    Pending,
    Confirmed,
    Failed
}
