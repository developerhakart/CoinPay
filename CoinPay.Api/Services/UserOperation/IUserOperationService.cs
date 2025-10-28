namespace CoinPay.Api.Services.UserOperation;

/// <summary>
/// Service for constructing and submitting ERC-4337 UserOperations
/// </summary>
public interface IUserOperationService
{
    /// <summary>
    /// Construct a UserOperation for ERC-20 token transfer
    /// </summary>
    Task<UserOperationDto> ConstructTransferOperationAsync(
        string fromAddress,
        string toAddress,
        string tokenAddress,
        decimal amount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Submit UserOperation to Circle bundler
    /// </summary>
    Task<string> SubmitUserOperationAsync(UserOperationDto userOp, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get UserOperation receipt from bundler
    /// </summary>
    Task<UserOperationReceipt?> GetReceiptAsync(string userOpHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Estimate gas for a UserOperation
    /// </summary>
    Task<GasEstimate> EstimateGasAsync(UserOperationDto userOp, CancellationToken cancellationToken = default);
}

/// <summary>
/// ERC-4337 UserOperation structure
/// </summary>
public class UserOperationDto
{
    public string Sender { get; set; } = string.Empty;
    public string Nonce { get; set; } = "0x0";
    public string InitCode { get; set; } = "0x";
    public string CallData { get; set; } = string.Empty;
    public string CallGasLimit { get; set; } = string.Empty;
    public string VerificationGasLimit { get; set; } = string.Empty;
    public string PreVerificationGas { get; set; } = string.Empty;
    public string MaxFeePerGas { get; set; } = string.Empty;
    public string MaxPriorityFeePerGas { get; set; } = string.Empty;
    public string PaymasterAndData { get; set; } = "0x";
    public string Signature { get; set; } = "0x";
}

/// <summary>
/// UserOperation receipt from bundler
/// </summary>
public class UserOperationReceipt
{
    public string UserOpHash { get; set; } = string.Empty;
    public string TransactionHash { get; set; } = string.Empty;
    public string EntryPoint { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
    public bool Success { get; set; }
    public decimal ActualGasCost { get; set; }
    public decimal ActualGasUsed { get; set; }
    public long BlockNumber { get; set; }
    public DateTime? BlockTimestamp { get; set; }
}

/// <summary>
/// Gas estimation for UserOperation
/// </summary>
public class GasEstimate
{
    public string CallGasLimit { get; set; } = string.Empty;
    public string VerificationGasLimit { get; set; } = string.Empty;
    public string PreVerificationGas { get; set; } = string.Empty;
    public string MaxFeePerGas { get; set; } = string.Empty;
    public string MaxPriorityFeePerGas { get; set; } = string.Empty;
}
