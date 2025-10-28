namespace CoinPay.Api.Services.Blockchain;

/// <summary>
/// Service for interacting with blockchain RPC nodes
/// </summary>
public interface IBlockchainRpcService
{
    /// <summary>
    /// Get USDC balance for a wallet address
    /// </summary>
    Task<decimal> GetUSDCBalanceAsync(string walletAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get native token balance (MATIC) for a wallet address
    /// </summary>
    Task<decimal> GetNativeBalanceAsync(string walletAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transaction receipt by transaction hash
    /// </summary>
    Task<TransactionReceipt?> GetTransactionReceiptAsync(string txHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get current gas price
    /// </summary>
    Task<decimal> GetGasPriceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get block number
    /// </summary>
    Task<long> GetBlockNumberAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Transaction receipt information
/// </summary>
public class TransactionReceipt
{
    public string TransactionHash { get; set; } = string.Empty;
    public string BlockHash { get; set; } = string.Empty;
    public long BlockNumber { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public decimal GasUsed { get; set; }
    public string Status { get; set; } = string.Empty; // "success" or "failed"
    public DateTime? Timestamp { get; set; }
}
