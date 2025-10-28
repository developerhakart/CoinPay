using CoinPay.Api.Data;
using CoinPay.Api.Services.Circle;
using CoinPay.Api.Services.Blockchain;
using CoinPay.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Services.Wallet;

public class WalletService : IWalletService
{
    private readonly ICircleService _circleService;
    private readonly IBlockchainRpcService _blockchainRpc;
    private readonly AppDbContext _dbContext;
    private readonly IWalletRepository _walletRepository;
    private readonly ILogger<WalletService> _logger;

    public WalletService(
        ICircleService circleService,
        IBlockchainRpcService blockchainRpc,
        AppDbContext dbContext,
        IWalletRepository walletRepository,
        ILogger<WalletService> logger)
    {
        _circleService = circleService;
        _blockchainRpc = blockchainRpc;
        _dbContext = dbContext;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task<WalletCreationResponse> CreateWalletAsync(int userId)
    {
        _logger.LogInformation("Creating wallet for user {UserId}", userId);

        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException($"User {userId} not found");

        // Check if user already has a wallet
        var existingWallet = await _walletRepository.GetByUserIdAsync(userId);
        if (existingWallet != null)
            throw new InvalidOperationException($"User already has a wallet: {existingWallet.Address}");

        if (string.IsNullOrEmpty(user.CircleUserId))
            throw new InvalidOperationException("User must complete registration first");

        // Create wallet via Circle
        var circleWallet = await _circleService.CreateWalletAsync(user.CircleUserId);

        // Create wallet entity
        var wallet = new Models.Wallet
        {
            UserId = userId,
            Address = circleWallet.Address,
            CircleWalletId = circleWallet.WalletId,
            Blockchain = circleWallet.Blockchain,
            WalletType = circleWallet.WalletType,
            Balance = circleWallet.Balance ?? 0m,
            BalanceCurrency = circleWallet.BalanceCurrency ?? "USDC",
            BalanceUpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow
        };

        // Save wallet to database
        await _walletRepository.CreateAsync(wallet);

        // Update user's wallet address for quick reference
        user.WalletAddress = wallet.Address;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Wallet created for user {UserId}: {WalletAddress}", userId, wallet.Address);

        return new WalletCreationResponse
        {
            WalletAddress = wallet.Address,
            WalletId = wallet.CircleWalletId,
            Blockchain = wallet.Blockchain,
            CreatedAt = wallet.CreatedAt
        };
    }

    public async Task<WalletBalanceResponse> GetWalletBalanceAsync(string walletAddress)
    {
        _logger.LogInformation("Getting balance for wallet {WalletAddress}", walletAddress);

        // Get wallet from database
        var wallet = await _walletRepository.GetByAddressAsync(walletAddress);
        if (wallet == null)
        {
            _logger.LogWarning("Wallet not found: {WalletAddress}", walletAddress);
            throw new InvalidOperationException($"Wallet {walletAddress} not found");
        }

        // Check if balance is cached (within last 30 seconds)
        if (wallet.BalanceUpdatedAt.HasValue &&
            (DateTime.UtcNow - wallet.BalanceUpdatedAt.Value).TotalSeconds < 30)
        {
            _logger.LogDebug("Returning cached balance for {WalletAddress}: {Balance}", walletAddress, wallet.Balance);
            return new WalletBalanceResponse
            {
                WalletAddress = walletAddress,
                USDCBalance = wallet.Balance,
                Blockchain = wallet.Blockchain
            };
        }

        // Fetch fresh balance from blockchain
        var balance = await _blockchainRpc.GetUSDCBalanceAsync(walletAddress);

        // Update cached balance in database
        wallet.Balance = balance;
        wallet.BalanceUpdatedAt = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);

        _logger.LogInformation("Balance updated for {WalletAddress}: {Balance} USDC", walletAddress, balance);

        return new WalletBalanceResponse
        {
            WalletAddress = walletAddress,
            USDCBalance = balance,
            Blockchain = wallet.Blockchain
        };
    }

    public Task<TransferResponse> TransferUSDCAsync(TransferRequest request)
    {
        _logger.LogInformation(
            "Initiating transfer from {From} to {To}, Amount: {Amount}",
            request.FromWalletAddress,
            request.ToWalletAddress,
            request.Amount);

        // For MVP, return mock response
        // In production, call Circle API to initiate transfer

        var transactionId = $"TXN{DateTime.UtcNow.Ticks}";

        return Task.FromResult(new TransferResponse
        {
            TransactionId = transactionId,
            Status = "Pending",
            Amount = request.Amount,
            FromAddress = request.FromWalletAddress,
            ToAddress = request.ToWalletAddress,
            InitiatedAt = DateTime.UtcNow
        });
    }

    public Task<TransactionStatusResponse> GetTransactionStatusAsync(string transactionId)
    {
        _logger.LogInformation("Getting status for transaction {TransactionId}", transactionId);

        // For MVP, return mock status
        // In production, query Circle API or blockchain

        return Task.FromResult(new TransactionStatusResponse
        {
            TransactionId = transactionId,
            Status = "Completed",
            TxHash = "0x" + Guid.NewGuid().ToString("N"),
            CompletedAt = DateTime.UtcNow
        });
    }

    public async Task<List<TransactionHistoryItem>> GetTransactionHistoryAsync(string walletAddress, int limit = 20)
    {
        _logger.LogInformation("Getting transaction history for wallet {WalletAddress}", walletAddress);

        // For MVP, return mock transaction history
        // In production, query Circle API or blockchain explorer

        var mockHistory = new List<TransactionHistoryItem>
        {
            new TransactionHistoryItem
            {
                TransactionId = "TXN001",
                TxHash = "0x" + Guid.NewGuid().ToString("N"),
                Type = "Receive",
                Amount = 100.00m,
                FromAddress = "0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb",
                ToAddress = walletAddress,
                Status = "Completed",
                Timestamp = DateTime.UtcNow.AddHours(-2)
            },
            new TransactionHistoryItem
            {
                TransactionId = "TXN002",
                TxHash = "0x" + Guid.NewGuid().ToString("N"),
                Type = "Send",
                Amount = 25.50m,
                FromAddress = walletAddress,
                ToAddress = "0x8E23Ee67d1332aD560396262C48ffbB273f626",
                Status = "Completed",
                Timestamp = DateTime.UtcNow.AddHours(-5)
            },
            new TransactionHistoryItem
            {
                TransactionId = "TXN003",
                TxHash = "0x" + Guid.NewGuid().ToString("N"),
                Type = "Receive",
                Amount = 50.00m,
                FromAddress = "0x3fC91A3afd70395Cd496C647d5a6CC9D4B2b7FAD",
                ToAddress = walletAddress,
                Status = "Completed",
                Timestamp = DateTime.UtcNow.AddDays(-1)
            }
        };

        return await Task.FromResult(mockHistory.Take(limit).ToList());
    }
}
