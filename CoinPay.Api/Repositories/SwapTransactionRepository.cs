using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository implementation for swap transactions
/// </summary>
public class SwapTransactionRepository : ISwapTransactionRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<SwapTransactionRepository> _logger;

    public SwapTransactionRepository(
        AppDbContext context,
        ILogger<SwapTransactionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SwapTransaction> CreateAsync(SwapTransaction transaction)
    {
        transaction.Id = Guid.NewGuid();
        transaction.CreatedAt = DateTime.UtcNow;
        transaction.UpdatedAt = DateTime.UtcNow;

        _context.SwapTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Created swap transaction: Id={Id}, User={UserId}, {From} -> {To}, Amount={Amount}",
            transaction.Id,
            transaction.UserId,
            transaction.FromTokenSymbol,
            transaction.ToTokenSymbol,
            transaction.FromAmount);

        return transaction;
    }

    public async Task<SwapTransaction?> GetByIdAsync(Guid id)
    {
        return await _context.SwapTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<SwapTransaction?> GetByTransactionHashAsync(string txHash)
    {
        return await _context.SwapTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.TransactionHash == txHash);
    }

    public async Task<List<SwapTransaction>> GetByUserIdAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20,
        SwapStatus? status = null)
    {
        var query = _context.SwapTransactions
            .AsNoTracking()
            .Where(s => s.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        var swaps = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return swaps;
    }

    public async Task<List<SwapTransaction>> GetByWalletAddressAsync(
        string walletAddress,
        int page = 1,
        int pageSize = 20)
    {
        var swaps = await _context.SwapTransactions
            .AsNoTracking()
            .Where(s => s.WalletAddress == walletAddress)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return swaps;
    }

    public async Task UpdateAsync(SwapTransaction transaction)
    {
        transaction.UpdatedAt = DateTime.UtcNow;

        _context.SwapTransactions.Update(transaction);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Updated swap transaction: Id={Id}, Status={Status}",
            transaction.Id,
            transaction.Status);
    }

    public async Task<int> GetSwapCountByUserAsync(Guid userId, SwapStatus? status = null)
    {
        var query = _context.SwapTransactions
            .Where(s => s.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        return await query.CountAsync();
    }

    public async Task<decimal> GetTotalVolumeByUserAsync(Guid userId)
    {
        // Calculate total volume in USD equivalent
        // For simplicity, assuming USDC amounts are USD equivalent
        var totalVolume = await _context.SwapTransactions
            .Where(s => s.UserId == userId && s.Status == SwapStatus.Confirmed)
            .Where(s => s.FromTokenSymbol == "USDC")
            .SumAsync(s => s.FromAmount);

        // For non-USDC swaps, would need to convert to USD using exchange rates
        // This is a simplified implementation for MVP

        return totalVolume;
    }
}
