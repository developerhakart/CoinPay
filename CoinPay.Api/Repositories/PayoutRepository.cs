using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository for payout transaction operations
/// </summary>
public class PayoutRepository : IPayoutRepository
{
    private readonly AppDbContext _context;

    public PayoutRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PayoutTransaction>> GetAllByUserIdAsync(int userId, int? limit = null, int? offset = null)
    {
        var query = _context.PayoutTransactions
            .Include(p => p.BankAccount)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.InitiatedAt);

        if (offset.HasValue)
        {
            query = (IOrderedQueryable<PayoutTransaction>)query.Skip(offset.Value);
        }

        if (limit.HasValue)
        {
            query = (IOrderedQueryable<PayoutTransaction>)query.Take(limit.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<PayoutTransaction?> GetByIdAsync(Guid id)
    {
        return await _context.PayoutTransactions
            .Include(p => p.BankAccount)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PayoutTransaction?> GetByGatewayTransactionIdAsync(string gatewayTransactionId)
    {
        return await _context.PayoutTransactions
            .Include(p => p.BankAccount)
            .FirstOrDefaultAsync(p => p.GatewayTransactionId == gatewayTransactionId);
    }

    public async Task<IEnumerable<PayoutTransaction>> GetByStatusAsync(string status, int? limit = null)
    {
        var query = _context.PayoutTransactions
            .Include(p => p.BankAccount)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.InitiatedAt);

        if (limit.HasValue)
        {
            query = (IOrderedQueryable<PayoutTransaction>)query.Take(limit.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<PayoutTransaction> AddAsync(PayoutTransaction payout)
    {
        _context.PayoutTransactions.Add(payout);
        await _context.SaveChangesAsync();
        return payout;
    }

    public async Task<PayoutTransaction> UpdateAsync(PayoutTransaction payout)
    {
        _context.PayoutTransactions.Update(payout);
        await _context.SaveChangesAsync();
        return payout;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.PayoutTransactions.AnyAsync(p => p.Id == id);
    }

    public async Task<int> GetCountByUserIdAsync(int userId)
    {
        return await _context.PayoutTransactions.CountAsync(p => p.UserId == userId);
    }

    public async Task<IEnumerable<PayoutTransaction>> GetByBankAccountIdAsync(Guid bankAccountId)
    {
        return await _context.PayoutTransactions
            .Where(p => p.BankAccountId == bankAccountId)
            .OrderByDescending(p => p.InitiatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasPendingPayoutsAsync(Guid bankAccountId)
    {
        return await _context.PayoutTransactions
            .AnyAsync(p => p.BankAccountId == bankAccountId &&
                          (p.Status == "pending" || p.Status == "processing"));
    }
}
