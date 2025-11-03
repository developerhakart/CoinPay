namespace CoinPay.Api.Services.Circle.Models;

/// <summary>
/// Request to initiate a transaction challenge for user authorization
/// </summary>
public class CircleTransactionChallengeRequest
{
    /// <summary>
    /// Circle user ID initiating the transaction
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Wallet ID to send from
    /// </summary>
    public string WalletId { get; set; } = string.Empty;

    /// <summary>
    /// Blockchain network (e.g., "MATIC-AMOY")
    /// </summary>
    public string Blockchain { get; set; } = "MATIC-AMOY";

    /// <summary>
    /// Transaction amounts and details
    /// </summary>
    public List<CircleTransactionAmount> Amounts { get; set; } = new();

    /// <summary>
    /// Recipient wallet address
    /// </summary>
    public string DestinationAddress { get; set; } = string.Empty;

    /// <summary>
    /// Token address for ERC-20 transfers (null for native currency)
    /// </summary>
    public string? TokenAddress { get; set; }

    /// <summary>
    /// Fee level: LOW, MEDIUM, HIGH
    /// </summary>
    public string FeeLevel { get; set; } = "MEDIUM";
}

/// <summary>
/// Transaction amount details
/// </summary>
public class CircleTransactionAmount
{
    /// <summary>
    /// Amount to transfer
    /// </summary>
    public string Amount { get; set; } = string.Empty;

    /// <summary>
    /// Token address (null for native currency like POL)
    /// </summary>
    public string? Token { get; set; }
}

/// <summary>
/// Response from Circle transaction challenge initiation
/// </summary>
public class CircleTransactionChallengeResponse
{
    /// <summary>
    /// Challenge ID for tracking
    /// </summary>
    public string ChallengeId { get; set; } = string.Empty;

    /// <summary>
    /// WebAuthn challenge data for passkey signing
    /// </summary>
    public string Challenge { get; set; } = string.Empty;

    /// <summary>
    /// Relying party ID
    /// </summary>
    public string RpId { get; set; } = string.Empty;

    /// <summary>
    /// User verification requirement
    /// </summary>
    public string UserVerification { get; set; } = string.Empty;
}

/// <summary>
/// Request to execute a transaction after passkey authorization
/// </summary>
public class CircleTransactionExecuteRequest
{
    /// <summary>
    /// Circle user ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Challenge ID from initiation
    /// </summary>
    public string ChallengeId { get; set; } = string.Empty;

    /// <summary>
    /// Signed challenge from passkey
    /// </summary>
    public string Signature { get; set; } = string.Empty;

    /// <summary>
    /// Authenticator data
    /// </summary>
    public string AuthenticatorData { get; set; } = string.Empty;

    /// <summary>
    /// Client data JSON
    /// </summary>
    public string ClientDataJson { get; set; } = string.Empty;
}

/// <summary>
/// Response from Circle transaction execution
/// </summary>
public class CircleTransactionResponse
{
    /// <summary>
    /// Transaction ID
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Transaction hash on blockchain (once mined)
    /// </summary>
    public string? TxHash { get; set; }

    /// <summary>
    /// Transaction status: PENDING, CONFIRMED, FAILED
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Blockchain network
    /// </summary>
    public string Blockchain { get; set; } = string.Empty;

    /// <summary>
    /// From address
    /// </summary>
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// To address
    /// </summary>
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Amount transferred
    /// </summary>
    public string Amount { get; set; } = string.Empty;

    /// <summary>
    /// Token address (null for native currency)
    /// </summary>
    public string? TokenAddress { get; set; }

    /// <summary>
    /// Transaction creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request for developer-controlled wallet transfer (no passkey required)
/// Uses Circle's Developer-Controlled Wallets API
/// </summary>
public class CircleDeveloperTransferRequest
{
    /// <summary>
    /// Idempotency key for duplicate prevention
    /// </summary>
    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Entity secret ciphertext for authorization
    /// </summary>
    public string EntitySecretCiphertext { get; set; } = string.Empty;

    /// <summary>
    /// Wallet ID to send from
    /// </summary>
    public string WalletId { get; set; } = string.Empty;

    /// <summary>
    /// Destination wallet address
    /// </summary>
    public string DestinationAddress { get; set; } = string.Empty;

    /// <summary>
    /// Blockchain network (e.g., "MATIC-AMOY")
    /// </summary>
    public string Blockchain { get; set; } = "MATIC-AMOY";

    /// <summary>
    /// Transaction amounts (array of strings)
    /// </summary>
    public List<string> Amounts { get; set; } = new();

    /// <summary>
    /// Fee level: LOW, MEDIUM, HIGH
    /// </summary>
    public string FeeLevel { get; set; } = "MEDIUM";

    /// <summary>
    /// Token address for ERC-20 transfers (null/empty for native currency like POL)
    /// </summary>
    public string? TokenAddress { get; set; }
}
