using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository implementation for investment positions
/// </summary>
public class InvestmentRepository : IInvestmentRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<InvestmentRepository> _logger;

    public InvestmentRepository(
        AppDbContext context,
        ILogger<InvestmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<InvestmentPosition?> GetByIdAsync(Guid id)
    {
        return await _context.InvestmentPositions
            .Include(i => i.Transactions)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<InvestmentPosition>> GetByUserIdAsync(Guid userId)
    {
        return await _context.InvestmentPositions
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<InvestmentPosition>> GetActivePositionsAsync()
    {
        return await _context.InvestmentPositions
            .Where(i => i.Status == InvestmentStatus.Active)
            .ToListAsync();
    }

    public async Task<List<InvestmentPosition>> GetActivePositionsByUserAsync(Guid userId)
    {
        return await _context.InvestmentPositions
            .Where(i => i.UserId == userId && i.Status == InvestmentStatus.Active)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<InvestmentPosition> CreateAsync(InvestmentPosition position)
    {
        position.Id = Guid.NewGuid();
        position.CreatedAt = DateTime.UtcNow;
        position.UpdatedAt = DateTime.UtcNow;
        position.CurrentValue = position.PrincipalAmount;
        position.AccruedRewards = 0;

        _context.InvestmentPositions.Add(position);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created investment position {PositionId} for user {UserId}",
            position.Id, position.UserId);

        return position;
    }

    public async Task<InvestmentPosition> UpdateAsync(InvestmentPosition position)
    {
        position.UpdatedAt = DateTime.UtcNow;
        _context.InvestmentPositions.Update(position);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated investment position {PositionId}", position.Id);

        return position;
    }

    public async Task DeleteAsync(Guid id)
    {
        var position = await GetByIdAsync(id);
        if (position != null)
        {
            _context.InvestmentPositions.Remove(position);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted investment position {PositionId}", id);
        }
    }

    public async Task<InvestmentTransaction> CreateTransactionAsync(InvestmentTransaction transaction)
    {
        transaction.Id = Guid.NewGuid();
        transaction.CreatedAt = DateTime.UtcNow;
        transaction.UpdatedAt = DateTime.UtcNow;

        _context.InvestmentTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created investment transaction {TransactionId} for position {PositionId}",
            transaction.Id, transaction.InvestmentPositionId);

        return transaction;
    }

    public async Task<List<InvestmentTransaction>> GetTransactionsByPositionIdAsync(Guid positionId)
    {
        return await _context.InvestmentTransactions
            .Where(t => t.InvestmentPositionId == positionId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<InvestmentTransaction>> GetTransactionsByUserIdAsync(Guid userId)
    {
        return await _context.InvestmentTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
