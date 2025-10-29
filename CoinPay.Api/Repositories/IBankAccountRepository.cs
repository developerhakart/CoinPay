using CoinPay.Api.Models;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository interface for bank account operations
/// </summary>
public interface IBankAccountRepository
{
    /// <summary>
    /// Get all bank accounts for a user
    /// </summary>
    Task<IEnumerable<BankAccount>> GetAllByUserIdAsync(int userId);

    /// <summary>
    /// Get bank account by ID
    /// </summary>
    Task<BankAccount?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get user's primary bank account
    /// </summary>
    Task<BankAccount?> GetPrimaryByUserIdAsync(int userId);

    /// <summary>
    /// Add new bank account
    /// </summary>
    Task<BankAccount> AddAsync(BankAccount bankAccount);

    /// <summary>
    /// Update existing bank account
    /// </summary>
    Task<BankAccount> UpdateAsync(BankAccount bankAccount);

    /// <summary>
    /// Delete bank account (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Check if bank account exists and belongs to user
    /// </summary>
    Task<bool> ExistsForUserAsync(Guid id, int userId);

    /// <summary>
    /// Check if user has any bank accounts
    /// </summary>
    Task<bool> HasBankAccountsAsync(int userId);

    /// <summary>
    /// Get count of bank accounts for user
    /// </summary>
    Task<int> GetCountByUserIdAsync(int userId);
}
