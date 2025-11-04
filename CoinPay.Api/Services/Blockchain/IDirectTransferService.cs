namespace CoinPay.Api.Services.Blockchain;

/// <summary>
/// Service for executing direct blockchain transfers using private keys
/// WARNING: For testing/development only. Production should use Circle's custody solution.
/// </summary>
public interface IDirectTransferService
{
    /// <summary>
    /// Send native POL/MATIC to an address
    /// </summary>
    /// <param name="toAddress">Recipient address</param>
    /// <param name="amountInMatic">Amount in MATIC/POL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction hash</returns>
    Task<DirectTransferResult> SendNativeAsync(string toAddress, decimal amountInMatic, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send USDC (ERC-20) to an address
    /// </summary>
    /// <param name="toAddress">Recipient address</param>
    /// <param name="amountInUsdc">Amount in USDC</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction hash</returns>
    Task<DirectTransferResult> SendUsdcAsync(string toAddress, decimal amountInUsdc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the test wallet address
    /// </summary>
    string GetTestWalletAddress();
}

/// <summary>
/// Result of a direct blockchain transfer
/// </summary>
public class DirectTransferResult
{
    /// <summary>
    /// Transaction hash on blockchain
    /// </summary>
    public string TxHash { get; set; } = string.Empty;

    /// <summary>
    /// From address
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// To address
    /// </summary>
    public string ToAddress { get; set; } = string.Empty;

    /// <summary>
    /// Amount sent
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency (POL or USDC)
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Transaction status (Pending, Confirmed, Failed)
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gas used for transaction
    /// </summary>
    public decimal GasUsed { get; set; }

    /// <summary>
    /// Timestamp when transaction was sent
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
