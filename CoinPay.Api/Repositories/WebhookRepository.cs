using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository implementation for webhook operations
/// </summary>
public class WebhookRepository : IWebhookRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<WebhookRepository> _logger;

    public WebhookRepository(AppDbContext context, ILogger<WebhookRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WebhookRegistration> CreateAsync(WebhookRegistration webhook, CancellationToken cancellationToken = default)
    {
        webhook.CreatedAt = DateTime.UtcNow;
        webhook.UpdatedAt = DateTime.UtcNow;

        _context.WebhookRegistrations.Add(webhook);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created webhook {WebhookId} for user {UserId}", webhook.Id, webhook.UserId);

        return webhook;
    }

    public async Task<WebhookRegistration?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.WebhookRegistrations
            .Include(w => w.DeliveryLogs.OrderByDescending(l => l.Timestamp).Take(10))
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<List<WebhookRegistration>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.WebhookRegistrations
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WebhookRegistration>> GetActiveWebhooksForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.WebhookRegistrations
            .Where(w => w.UserId == userId && w.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<WebhookRegistration> UpdateAsync(WebhookRegistration webhook, CancellationToken cancellationToken = default)
    {
        webhook.UpdatedAt = DateTime.UtcNow;

        _context.WebhookRegistrations.Update(webhook);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated webhook {WebhookId}", webhook.Id);

        return webhook;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var webhook = await _context.WebhookRegistrations.FindAsync(new object[] { id }, cancellationToken);

        if (webhook != null)
        {
            _context.WebhookRegistrations.Remove(webhook);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted webhook {WebhookId}", id);
        }
    }

    public async Task<WebhookDeliveryLog> LogDeliveryAsync(WebhookDeliveryLog log, CancellationToken cancellationToken = default)
    {
        log.Timestamp = DateTime.UtcNow;

        _context.WebhookDeliveryLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);

        return log;
    }

    public async Task<List<WebhookDeliveryLog>> GetDeliveryLogsAsync(int webhookId, int limit = 100, CancellationToken cancellationToken = default)
    {
        return await _context.WebhookDeliveryLogs
            .Where(l => l.WebhookId == webhookId)
            .OrderByDescending(l => l.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WebhookDeliveryLog>> GetDeliveryLogsByTransactionAsync(int transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.WebhookDeliveryLogs
            .Where(l => l.TransactionId == transactionId)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
