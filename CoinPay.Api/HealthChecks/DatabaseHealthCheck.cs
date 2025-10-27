using Microsoft.Extensions.Diagnostics.HealthChecks;
using CoinPay.Api.Data;

namespace CoinPay.Api.HealthChecks;

/// <summary>
/// Health check for database connectivity.
/// Verifies that the application can successfully connect to the PostgreSQL database.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    /// <summary>
    /// Initializes a new instance of the DatabaseHealthCheck class.
    /// </summary>
    /// <param name="context">The database context to check connectivity.</param>
    /// <param name="logger">Logger for health check operations.</param>
    public DatabaseHealthCheck(AppDbContext context, ILogger<DatabaseHealthCheck> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Performs the health check by attempting to connect to the database.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A health check result indicating the database status.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Performing database health check");

            // Attempt to connect to the database
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

            if (canConnect)
            {
                _logger.LogDebug("Database health check passed");
                return HealthCheckResult.Healthy("Database connection is healthy");
            }
            else
            {
                _logger.LogWarning("Database health check failed: Cannot connect to database");
                return HealthCheckResult.Unhealthy("Cannot connect to database");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed with exception");
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}
