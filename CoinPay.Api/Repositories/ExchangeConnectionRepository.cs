using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository implementation for exchange connections
/// </summary>
public class ExchangeConnectionRepository : IExchangeConnectionRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ExchangeConnectionRepository> _logger;

    public ExchangeConnectionRepository(
        AppDbContext context,
        ILogger<ExchangeConnectionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ExchangeConnection?> GetByIdAsync(Guid id)
    {
        return await _context.ExchangeConnections
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<ExchangeConnection?> GetByUserAndExchangeAsync(Guid userId, string exchangeName)
    {
        return await _context.ExchangeConnections
            .FirstOrDefaultAsync(e => e.UserId == userId && e.ExchangeName == exchangeName);
    }

    public async Task<List<ExchangeConnection>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ExchangeConnections
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<ExchangeConnection> CreateAsync(ExchangeConnection connection)
    {
        connection.Id = Guid.NewGuid();
        connection.CreatedAt = DateTime.UtcNow;
        connection.UpdatedAt = DateTime.UtcNow;

        _context.ExchangeConnections.Add(connection);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created exchange connection {ConnectionId} for user {UserId}",
            connection.Id, connection.UserId);

        return connection;
    }

    public async Task<ExchangeConnection> UpdateAsync(ExchangeConnection connection)
    {
        connection.UpdatedAt = DateTime.UtcNow;
        _context.ExchangeConnections.Update(connection);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated exchange connection {ConnectionId}", connection.Id);

        return connection;
    }

    public async Task DeleteAsync(Guid id)
    {
        var connection = await GetByIdAsync(id);
        if (connection != null)
        {
            _context.ExchangeConnections.Remove(connection);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted exchange connection {ConnectionId}", id);
        }
    }

    public async Task<bool> ExistsAsync(Guid userId, string exchangeName)
    {
        return await _context.ExchangeConnections
            .AnyAsync(e => e.UserId == userId && e.ExchangeName == exchangeName);
    }
}
