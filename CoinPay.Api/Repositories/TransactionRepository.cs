using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository implementation for blockchain transaction operations
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<TransactionRepository> _logger;

    public TransactionRepository(AppDbContext context, ILogger<TransactionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BlockchainTransaction> CreateAsync(BlockchainTransaction transaction, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new blockchain transaction for wallet {WalletId}", transaction.WalletId);

        transaction.CreatedAt = DateTime.UtcNow;
        transaction.SubmittedAt = DateTime.UtcNow;

        _context.BlockchainTransactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Blockchain transaction created with ID {Id} and UserOpHash {UserOpHash}",
            transaction.Id, transaction.UserOpHash);

        return transaction;
    }

    public async Task<BlockchainTransaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.BlockchainTransactions
            .Include(t => t.Wallet)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<BlockchainTransaction?> GetByUserOpHashAsync(string userOpHash, CancellationToken cancellationToken = default)
    {
        return await _context.BlockchainTransactions
            .Include(t => t.Wallet)
            .FirstOrDefaultAsync(t => t.UserOpHash == userOpHash, cancellationToken);
    }

    public async Task<BlockchainTransaction?> GetByTransactionHashAsync(string txHash, CancellationToken cancellationToken = default)
    {
        return await _context.BlockchainTransactions
            .Include(t => t.Wallet)
            .FirstOrDefaultAsync(t => t.TransactionHash == txHash, cancellationToken);
    }

    public async Task<List<BlockchainTransaction>> GetByWalletIdAsync(int walletId, int limit = 20, CancellationToken cancellationToken = default)
    {
        return await _context.BlockchainTransactions
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BlockchainTransaction>> GetByWalletAddressAsync(string address, int limit = 20, CancellationToken cancellationToken = default)
    {
        return await _context.BlockchainTransactions
            .Include(t => t.Wallet)
            .Where(t => t.FromAddress == address || t.ToAddress == address)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(int transactionId, TransactionStatus status, string? txHash = null, CancellationToken cancellationToken = default)
    {
        var transaction = await _context.BlockchainTransactions.FindAsync(new object[] { transactionId }, cancellationToken);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction with ID {TransactionId} not found for status update", transactionId);
            return;
        }

        transaction.Status = status;

        if (!string.IsNullOrEmpty(txHash))
        {
            transaction.TransactionHash = txHash;
        }

        if (status == TransactionStatus.Confirmed)
        {
            transaction.ConfirmedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Transaction {TransactionId} status updated to {Status}", transactionId, status);
    }

    public async Task UpdateWithReceiptAsync(int transactionId, string txHash, long blockNumber, decimal gasUsed, CancellationToken cancellationToken = default)
    {
        var transaction = await _context.BlockchainTransactions.FindAsync(new object[] { transactionId }, cancellationToken);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction with ID {TransactionId} not found for receipt update", transactionId);
            return;
        }

        transaction.TransactionHash = txHash;
        transaction.BlockNumber = blockNumber;
        transaction.GasUsed = gasUsed;
        transaction.Status = TransactionStatus.Confirmed;
        transaction.ConfirmedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Transaction {TransactionId} updated with receipt. TxHash: {TxHash}, Block: {BlockNumber}",
            transactionId, txHash, blockNumber);
    }

    public async Task<bool> ExistsAsync(string userOpHash, CancellationToken cancellationToken = default)
    {
        return await _context.BlockchainTransactions
            .AnyAsync(t => t.UserOpHash == userOpHash, cancellationToken);
    }

    public async Task<List<BlockchainTransaction>> GetPendingByWalletIdAsync(int walletId, CancellationToken cancellationToken = default)
    {
        return await _context.BlockchainTransactions
            .Where(t => t.WalletId == walletId && t.Status == TransactionStatus.Pending)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<BlockchainTransaction> transactions, int totalCount)> GetHistoryAsync(
        int walletId,
        int page = 1,
        int pageSize = 20,
        string? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        string sortBy = "CreatedAt",
        bool sortDescending = true,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Fetching transaction history for wallet {WalletId}: Page={Page}, PageSize={PageSize}, Status={Status}, SortBy={SortBy}",
            walletId, page, pageSize, status, sortBy);

        // Start with base query
        var query = _context.BlockchainTransactions
            .Include(t => t.Wallet)
            .Where(t => t.WalletId == walletId);

        // Apply status filter
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<TransactionStatus>(status, true, out var statusEnum))
        {
            query = query.Where(t => t.Status == statusEnum);
        }

        // Apply date range filter
        if (startDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= endDate.Value);
        }

        // Apply amount range filter
        if (minAmount.HasValue)
        {
            query = query.Where(t => t.AmountDecimal >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(t => t.AmountDecimal <= maxAmount.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "amount" => sortDescending
                ? query.OrderByDescending(t => t.AmountDecimal)
                : query.OrderBy(t => t.AmountDecimal),
            "status" => sortDescending
                ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),
            "confirmedat" => sortDescending
                ? query.OrderByDescending(t => t.ConfirmedAt)
                : query.OrderBy(t => t.ConfirmedAt),
            _ => sortDescending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt)
        };

        // Apply pagination
        var transactions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} transactions out of {Total} total for wallet {WalletId}",
            transactions.Count, totalCount, walletId);

        return (transactions, totalCount);
    }
}
