using CoinPay.Api.Models;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository interface for payout transaction operations
/// </summary>
public interface IPayoutRepository
{
    /// <summary>
    /// Get all payouts for a user
    /// </summary>
    Task<IEnumerable<PayoutTransaction>> GetAllByUserIdAsync(int userId, int? limit = null, int? offset = null);

    /// <summary>
    /// Get payout by ID
    /// </summary>
    Task<PayoutTransaction?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get payout by gateway transaction ID
    /// </summary>
    Task<PayoutTransaction?> GetByGatewayTransactionIdAsync(string gatewayTransactionId);

    /// <summary>
    /// Get payouts by status
    /// </summary>
    Task<IEnumerable<PayoutTransaction>> GetByStatusAsync(string status, int? limit = null);

    /// <summary>
    /// Add new payout
    /// </summary>
    Task<PayoutTransaction> AddAsync(PayoutTransaction payout);

    /// <summary>
    /// Update payout
    /// </summary>
    Task<PayoutTransaction> UpdateAsync(PayoutTransaction payout);

    /// <summary>
    /// Check if payout exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Get payout count for user
    /// </summary>
    Task<int> GetCountByUserIdAsync(int userId);

    /// <summary>
    /// Get payouts by bank account ID
    /// </summary>
    Task<IEnumerable<PayoutTransaction>> GetByBankAccountIdAsync(Guid bankAccountId);

    /// <summary>
    /// Check if bank account has pending payouts
    /// </summary>
    Task<bool> HasPendingPayoutsAsync(Guid bankAccountId);
}
