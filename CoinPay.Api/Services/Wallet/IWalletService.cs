namespace CoinPay.Api.Services.Wallet;

public interface IWalletService
{
    Task<WalletCreationResponse> CreateWalletAsync(int userId);
    Task<WalletBalanceResponse> GetWalletBalanceAsync(string walletAddress);
    Task<TransferResponse> TransferUSDCAsync(TransferRequest request);
    Task<TransactionStatusResponse> GetTransactionStatusAsync(string transactionId);
    Task<List<TransactionHistoryItem>> GetTransactionHistoryAsync(string walletAddress, int limit = 20);
}

public class WalletCreationResponse
{
    public string WalletAddress { get; set; } = string.Empty;
    public string WalletId { get; set; } = string.Empty;
    public string Blockchain { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class WalletBalanceResponse
{
    public string WalletAddress { get; set; } = string.Empty;
    public decimal USDCBalance { get; set; }
    public string Blockchain { get; set; } = string.Empty;
}

public class TransferRequest
{
    public string FromWalletAddress { get; set; } = string.Empty;
    public string ToWalletAddress { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Memo { get; set; }
}

public class TransferResponse
{
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public DateTime InitiatedAt { get; set; }
}

public class TransactionStatusResponse
{
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TxHash { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class TransactionHistoryItem
{
    public string TransactionId { get; set; } = string.Empty;
    public string TxHash { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Send" or "Receive"
    public decimal Amount { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
