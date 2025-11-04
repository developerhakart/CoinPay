using CoinPay.Api.Models;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository for exchange connection data access
/// </summary>
public interface IExchangeConnectionRepository
{
    Task<ExchangeConnection?> GetByIdAsync(Guid id);
    Task<ExchangeConnection?> GetByUserAndExchangeAsync(Guid userId, string exchangeName);
    Task<List<ExchangeConnection>> GetByUserIdAsync(Guid userId);
    Task<ExchangeConnection> CreateAsync(ExchangeConnection connection);
    Task<ExchangeConnection> UpdateAsync(ExchangeConnection connection);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid userId, string exchangeName);
}
