using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository implementation for bank account operations
/// </summary>
public class BankAccountRepository : IBankAccountRepository
{
    private readonly AppDbContext _context;

    public BankAccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BankAccount>> GetAllByUserIdAsync(int userId)
    {
        return await _context.BankAccounts
            .Where(b => b.UserId == userId && b.DeletedAt == null)
            .OrderByDescending(b => b.IsPrimary)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<BankAccount?> GetByIdAsync(Guid id)
    {
        return await _context.BankAccounts
            .Where(b => b.Id == id && b.DeletedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task<BankAccount?> GetPrimaryByUserIdAsync(int userId)
    {
        return await _context.BankAccounts
            .Where(b => b.UserId == userId && b.IsPrimary && b.DeletedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task<BankAccount> AddAsync(BankAccount bankAccount)
    {
        bankAccount.CreatedAt = DateTime.UtcNow;
        bankAccount.UpdatedAt = DateTime.UtcNow;

        // If this is set as primary, unset any existing primary
        if (bankAccount.IsPrimary)
        {
            var existingPrimary = await GetPrimaryByUserIdAsync(bankAccount.UserId);
            if (existingPrimary != null)
            {
                existingPrimary.IsPrimary = false;
                existingPrimary.UpdatedAt = DateTime.UtcNow;
            }
        }

        _context.BankAccounts.Add(bankAccount);
        await _context.SaveChangesAsync();
        return bankAccount;
    }

    public async Task<BankAccount> UpdateAsync(BankAccount bankAccount)
    {
        bankAccount.UpdatedAt = DateTime.UtcNow;

        // If this is being set as primary, unset any existing primary
        if (bankAccount.IsPrimary)
        {
            var existingPrimary = await GetPrimaryByUserIdAsync(bankAccount.UserId);
            if (existingPrimary != null && existingPrimary.Id != bankAccount.Id)
            {
                existingPrimary.IsPrimary = false;
                existingPrimary.UpdatedAt = DateTime.UtcNow;
            }
        }

        _context.BankAccounts.Update(bankAccount);
        await _context.SaveChangesAsync();
        return bankAccount;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var bankAccount = await GetByIdAsync(id);
        if (bankAccount == null)
            return false;

        // Soft delete
        bankAccount.DeletedAt = DateTime.UtcNow;
        bankAccount.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsForUserAsync(Guid id, int userId)
    {
        return await _context.BankAccounts
            .AnyAsync(b => b.Id == id && b.UserId == userId && b.DeletedAt == null);
    }

    public async Task<bool> HasBankAccountsAsync(int userId)
    {
        return await _context.BankAccounts
            .AnyAsync(b => b.UserId == userId && b.DeletedAt == null);
    }

    public async Task<int> GetCountByUserIdAsync(int userId)
    {
        return await _context.BankAccounts
            .CountAsync(b => b.UserId == userId && b.DeletedAt == null);
    }
}
