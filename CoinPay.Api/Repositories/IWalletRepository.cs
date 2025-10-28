using CoinPay.Api.Models;

namespace CoinPay.Api.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Wallet?> GetByAddressAsync(string address, CancellationToken cancellationToken = default);
    Task<Wallet?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Wallet> CreateAsync(Wallet wallet, CancellationToken cancellationToken = default);
    Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string address, CancellationToken cancellationToken = default);
}
