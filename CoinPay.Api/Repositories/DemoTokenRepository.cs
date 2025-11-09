using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository for managing demo token balances
/// </summary>
public class DemoTokenRepository : IDemoTokenRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<DemoTokenRepository> _logger;

    public DemoTokenRepository(AppDbContext context, ILogger<DemoTokenRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DemoTokenBalance?> GetByUserAndTokenAsync(int userId, string tokenSymbol)
    {
        return await _context.DemoTokenBalances
            .FirstOrDefaultAsync(b => b.UserId == userId && b.TokenSymbol == tokenSymbol);
    }

    public async Task<List<DemoTokenBalance>> GetByUserIdAsync(int userId)
    {
        return await _context.DemoTokenBalances
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.TokenSymbol)
            .ToListAsync();
    }

    public async Task<DemoTokenBalance> UpsertAsync(DemoTokenBalance balance)
    {
        var existing = await GetByUserAndTokenAsync(balance.UserId, balance.TokenSymbol);

        if (existing != null)
        {
            existing.Balance = balance.Balance;
            existing.TotalIssued = balance.TotalIssued;
            existing.TotalInvested = balance.TotalInvested;
            existing.IsActive = balance.IsActive;
            existing.LastUpdated = DateTime.UtcNow;
            existing.Notes = balance.Notes;

            _context.DemoTokenBalances.Update(existing);
        }
        else
        {
            balance.Id = Guid.NewGuid();
            balance.CreatedAt = DateTime.UtcNow;
            balance.LastUpdated = DateTime.UtcNow;
            await _context.DemoTokenBalances.AddAsync(balance);
        }

        await _context.SaveChangesAsync();
        return existing ?? balance;
    }

    public async Task<DemoTokenBalance> IssueDemoTokensAsync(int userId, string tokenSymbol, decimal amount)
    {
        var balance = await GetByUserAndTokenAsync(userId, tokenSymbol);

        if (balance == null)
        {
            balance = new DemoTokenBalance
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenSymbol = tokenSymbol,
                Balance = amount,
                TotalIssued = amount,
                TotalInvested = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            await _context.DemoTokenBalances.AddAsync(balance);
        }
        else
        {
            balance.Balance += amount;
            balance.TotalIssued += amount;
            balance.LastUpdated = DateTime.UtcNow;
            _context.DemoTokenBalances.Update(balance);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Issued {Amount} {TokenSymbol} demo tokens to user {UserId}",
            amount, tokenSymbol, userId);

        return balance;
    }

    public async Task<bool> DeductBalanceAsync(int userId, string tokenSymbol, decimal amount)
    {
        var balance = await GetByUserAndTokenAsync(userId, tokenSymbol);

        if (balance == null || balance.Balance < amount)
        {
            _logger.LogWarning("Insufficient demo token balance for user {UserId}. Token: {TokenSymbol}, Required: {Amount}, Available: {Balance}",
                userId, tokenSymbol, amount, balance?.Balance ?? 0);
            return false;
        }

        balance.Balance -= amount;
        balance.TotalInvested += amount;
        balance.LastUpdated = DateTime.UtcNow;

        _context.DemoTokenBalances.Update(balance);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deducted {Amount} {TokenSymbol} demo tokens from user {UserId}. Remaining: {Remaining}",
            amount, tokenSymbol, userId, balance.Balance);

        return true;
    }

    public async Task<bool> AddBalanceAsync(int userId, string tokenSymbol, decimal amount)
    {
        var balance = await GetByUserAndTokenAsync(userId, tokenSymbol);

        if (balance == null)
        {
            _logger.LogWarning("Cannot add to non-existent demo token balance for user {UserId}, token {TokenSymbol}",
                userId, tokenSymbol);
            return false;
        }

        balance.Balance += amount;
        balance.TotalInvested = Math.Max(0, balance.TotalInvested - amount); // Reduce invested amount
        balance.LastUpdated = DateTime.UtcNow;

        _context.DemoTokenBalances.Update(balance);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Added {Amount} {TokenSymbol} demo tokens to user {UserId}. New balance: {Balance}",
            amount, tokenSymbol, userId, balance.Balance);

        return true;
    }

    public async Task<bool> HasSufficientBalanceAsync(int userId, string tokenSymbol, decimal amount)
    {
        var balance = await GetByUserAndTokenAsync(userId, tokenSymbol);
        return balance != null && balance.Balance >= amount && balance.IsActive;
    }
}
