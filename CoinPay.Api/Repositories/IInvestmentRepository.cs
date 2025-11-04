using CoinPay.Api.Models;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository for investment position data access
/// </summary>
public interface IInvestmentRepository
{
    Task<InvestmentPosition?> GetByIdAsync(Guid id);
    Task<List<InvestmentPosition>> GetByUserIdAsync(Guid userId);
    Task<List<InvestmentPosition>> GetActivePositionsAsync();
    Task<List<InvestmentPosition>> GetActivePositionsByUserAsync(Guid userId);
    Task<InvestmentPosition> CreateAsync(InvestmentPosition position);
    Task<InvestmentPosition> UpdateAsync(InvestmentPosition position);
    Task DeleteAsync(Guid id);

    // Transaction operations
    Task<InvestmentTransaction> CreateTransactionAsync(InvestmentTransaction transaction);
    Task<List<InvestmentTransaction>> GetTransactionsByPositionIdAsync(Guid positionId);
    Task<List<InvestmentTransaction>> GetTransactionsByUserIdAsync(Guid userId);
}
