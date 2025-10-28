using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<WalletRepository> _logger;

    public WalletRepository(AppDbContext context, ILogger<WalletRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Wallet?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting wallet by ID: {WalletId}", id);
        return await _context.Wallets
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Wallet?> GetByAddressAsync(string address, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting wallet by address: {Address}", address);
        return await _context.Wallets
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.Address == address, cancellationToken);
    }

    public async Task<Wallet?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting wallet by user ID: {UserId}", userId);
        return await _context.Wallets
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task<Wallet> CreateAsync(Wallet wallet, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating wallet for user {UserId} with address {Address}", wallet.UserId, wallet.Address);

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Wallet created successfully: ID {WalletId}, Address {Address}", wallet.Id, wallet.Address);
        return wallet;
    }

    public async Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating wallet: {WalletId}", wallet.Id);

        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Wallet updated successfully: {WalletId}", wallet.Id);
    }

    public async Task<bool> ExistsAsync(string address, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets
            .AnyAsync(w => w.Address == address, cancellationToken);
    }
}
