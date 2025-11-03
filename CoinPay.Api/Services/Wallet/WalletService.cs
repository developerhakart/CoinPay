using CoinPay.Api.Data;
using CoinPay.Api.Services.Circle;
using CoinPay.Api.Services.Blockchain;
using CoinPay.Api.Services.Caching;
using CoinPay.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CoinPay.Api.Services.Wallet;

public class WalletService : IWalletService
{
    private readonly ICircleService _circleService;
    private readonly IBlockchainRpcService _blockchainRpc;
    private readonly AppDbContext _dbContext;
    private readonly IWalletRepository _walletRepository;
    private readonly ICachingService? _cachingService;
    private readonly ILogger<WalletService> _logger;
    private const int CacheTTLSeconds = 30;

    public WalletService(
        ICircleService circleService,
        IBlockchainRpcService blockchainRpc,
        AppDbContext dbContext,
        IWalletRepository walletRepository,
        ILogger<WalletService> logger,
        ICachingService? cachingService = null)
    {
        _circleService = circleService;
        _blockchainRpc = blockchainRpc;
        _dbContext = dbContext;
        _walletRepository = walletRepository;
        _cachingService = cachingService;
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

    public async Task<WalletBalanceResponse> GetWalletBalanceAsync(string walletAddress, bool forceRefresh = false)
    {
        _logger.LogInformation("Getting balance for wallet {WalletAddress}, ForceRefresh={ForceRefresh}",
            walletAddress, forceRefresh);

        // Get wallet from database
        var wallet = await _walletRepository.GetByAddressAsync(walletAddress);
        if (wallet == null)
        {
            _logger.LogInformation("Wallet not found in database: {WalletAddress}, fetching balance from blockchain", walletAddress);

            // Wallet not in our database - fetch real balance directly from blockchain
            try
            {
                var usdcBalance = await _blockchainRpc.GetUSDCBalanceAsync(walletAddress);
                var nativeBalance = await _blockchainRpc.GetNativeBalanceAsync(walletAddress);

                _logger.LogInformation("Blockchain balance for {WalletAddress}: {USDCBalance} USDC, {NativeBalance} POL",
                    walletAddress, usdcBalance, nativeBalance);

                return new WalletBalanceResponse
                {
                    WalletAddress = walletAddress,
                    USDCBalance = usdcBalance,
                    NativeBalance = nativeBalance,
                    Blockchain = "PolygonAmoy"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch blockchain balance for {WalletAddress}, returning 0", walletAddress);

                // Return 0 if blockchain query fails
                return new WalletBalanceResponse
                {
                    WalletAddress = walletAddress,
                    USDCBalance = 0,
                    NativeBalance = 0,
                    Blockchain = "PolygonAmoy"
                };
            }
        }

        var cacheKey = $"wallet:balance:{walletAddress}";

        // Try to get from Redis cache first (if not forcing refresh)
        if (!forceRefresh && _cachingService != null)
        {
            var cachedBalance = await _cachingService.GetAsync(cacheKey);
            if (cachedBalance != null)
            {
                try
                {
                    var balanceData = JsonSerializer.Deserialize<CachedBalanceData>(cachedBalance);
                    if (balanceData != null)
                    {
                        // Fetch native balance from blockchain (not cached to always show real-time POL)
                        var cachedNativeBalance = await _blockchainRpc.GetNativeBalanceAsync(walletAddress);

                        _logger.LogDebug("Returning cached USDC balance from Redis for {WalletAddress}: {USDCBalance} USDC, {NativeBalance} POL",
                            walletAddress, balanceData.Balance, cachedNativeBalance);

                        return new WalletBalanceResponse
                        {
                            WalletAddress = walletAddress,
                            USDCBalance = balanceData.Balance,
                            NativeBalance = cachedNativeBalance,
                            Blockchain = wallet.Blockchain
                        };
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize cached balance for {WalletAddress}", walletAddress);
                }
            }
        }

        // Fetch fresh balances from blockchain
        var freshUsdcBalance = await _blockchainRpc.GetUSDCBalanceAsync(walletAddress);
        var freshNativeBalance = await _blockchainRpc.GetNativeBalanceAsync(walletAddress);

        // Update database
        wallet.Balance = freshUsdcBalance;
        wallet.BalanceUpdatedAt = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);

        // Update Redis cache
        if (_cachingService != null)
        {
            var cacheData = new CachedBalanceData { Balance = freshUsdcBalance, UpdatedAt = DateTime.UtcNow };
            var serialized = JsonSerializer.Serialize(cacheData);
            await _cachingService.SetAsync(cacheKey, serialized, TimeSpan.FromSeconds(CacheTTLSeconds));
            _logger.LogDebug("Balance cached in Redis for {WalletAddress}: {USDCBalance} USDC, {NativeBalance} POL, TTL={TTL}s",
                walletAddress, freshUsdcBalance, freshNativeBalance, CacheTTLSeconds);
        }

        _logger.LogInformation("Balance updated for {WalletAddress}: {USDCBalance} USDC, {NativeBalance} POL",
            walletAddress, freshUsdcBalance, freshNativeBalance);

        return new WalletBalanceResponse
        {
            WalletAddress = walletAddress,
            USDCBalance = freshUsdcBalance,
            NativeBalance = freshNativeBalance,
            Blockchain = wallet.Blockchain
        };
    }

    /// <summary>
    /// Invalidates the balance cache for a wallet address
    /// </summary>
    public async Task InvalidateBalanceCacheAsync(string walletAddress)
    {
        if (_cachingService != null)
        {
            var cacheKey = $"wallet:balance:{walletAddress}";
            await _cachingService.RemoveAsync(cacheKey);
            _logger.LogInformation("Balance cache invalidated for {WalletAddress}", walletAddress);
        }
    }

    // Internal class for caching balance data
    private class CachedBalanceData
    {
        public decimal Balance { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public async Task<TransferResponse> TransferUSDCAsync(WalletTransferRequest request)
    {
        _logger.LogInformation(
            "Initiating transfer from {From} to {To}, Amount: {Amount}",
            request.FromWalletAddress,
            request.ToWalletAddress,
            request.Amount);

        // For MVP, return mock response
        // In production, call Circle API to initiate transfer

        var transactionId = $"TXN{DateTime.UtcNow.Ticks}";

        // Invalidate balance cache for both sender and receiver
        // Since the transfer is initiated, balances will change
        await InvalidateBalanceCacheAsync(request.FromWalletAddress);
        await InvalidateBalanceCacheAsync(request.ToWalletAddress);

        _logger.LogInformation("Balance caches invalidated for sender and receiver");

        return new TransferResponse
        {
            TransactionId = transactionId,
            Status = "Pending",
            Amount = request.Amount,
            FromAddress = request.FromWalletAddress,
            ToAddress = request.ToWalletAddress,
            InitiatedAt = DateTime.UtcNow
        };
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

    public async Task<Models.User?> GetUserByIdAsync(int userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }

    /// <summary>
    /// Deduct USDC balance from wallet (for payouts, etc.)
    /// This method checks balance first and throws if insufficient
    /// </summary>
    public async Task<decimal> DeductBalanceAsync(string walletAddress, decimal amount)
    {
        _logger.LogInformation("Deducting {Amount} USDC from wallet {WalletAddress}", amount, walletAddress);

        if (amount <= 0)
        {
            throw new ArgumentException("Deduction amount must be greater than zero", nameof(amount));
        }

        // Get wallet from database
        var wallet = await _walletRepository.GetByAddressAsync(walletAddress);
        if (wallet == null)
        {
            _logger.LogError("Wallet not found: {WalletAddress}", walletAddress);
            throw new InvalidOperationException($"Wallet {walletAddress} not found");
        }

        // Get current balance (force refresh to ensure accuracy)
        var balanceResult = await GetWalletBalanceAsync(walletAddress, forceRefresh: true);

        // Check if sufficient balance
        if (balanceResult.USDCBalance < amount)
        {
            _logger.LogWarning("Insufficient balance for wallet {WalletAddress}. Required: {Required}, Available: {Available}",
                walletAddress, amount, balanceResult.USDCBalance);
            throw new InvalidOperationException(
                $"Insufficient USDC balance. Required: {amount} USDC, Available: {balanceResult.USDCBalance} USDC");
        }

        // Deduct balance in database
        var newBalance = balanceResult.USDCBalance - amount;
        wallet.Balance = newBalance;
        wallet.BalanceUpdatedAt = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);

        // Invalidate cache to ensure fresh data on next query
        await InvalidateBalanceCacheAsync(walletAddress);

        _logger.LogInformation("Successfully deducted {Amount} USDC from wallet {WalletAddress}. New balance: {NewBalance}",
            amount, walletAddress, newBalance);

        return newBalance;
    }

    /// <summary>
    /// Refund USDC balance to wallet (for failed payouts, cancellations, etc.)
    /// </summary>
    public async Task RefundBalanceAsync(string walletAddress, decimal amount)
    {
        _logger.LogInformation("Refunding {Amount} USDC to wallet {WalletAddress}", amount, walletAddress);

        if (amount <= 0)
        {
            throw new ArgumentException("Refund amount must be greater than zero", nameof(amount));
        }

        // Get wallet from database
        var wallet = await _walletRepository.GetByAddressAsync(walletAddress);
        if (wallet == null)
        {
            _logger.LogError("Wallet not found: {WalletAddress}", walletAddress);
            throw new InvalidOperationException($"Wallet {walletAddress} not found");
        }

        // Add balance in database
        wallet.Balance += amount;
        wallet.BalanceUpdatedAt = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);

        // Invalidate cache to ensure fresh data on next query
        await InvalidateBalanceCacheAsync(walletAddress);

        _logger.LogInformation("Successfully refunded {Amount} USDC to wallet {WalletAddress}. New balance: {NewBalance}",
            amount, walletAddress, wallet.Balance);
    }
}
