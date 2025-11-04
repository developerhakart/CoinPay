using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using CoinPay.Api.Services.Investment;
using CoinPay.Api.Services.Exchange.WhiteBit;
using CoinPay.Api.Services.Encryption;

namespace CoinPay.Api.Services.BackgroundWorkers;

/// <summary>
/// Background service that syncs investment positions with WhiteBit exchange
/// Runs every 60 seconds to update positions, calculate rewards, and sync balances
/// </summary>
public class InvestmentPositionSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InvestmentPositionSyncService> _logger;
    private readonly TimeSpan _syncInterval = TimeSpan.FromSeconds(60);

    public InvestmentPositionSyncService(
        IServiceProvider serviceProvider,
        ILogger<InvestmentPositionSyncService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Investment Position Sync Service started");

        // Wait 30 seconds on startup before first sync
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncPositionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during position sync cycle");
            }

            try
            {
                await Task.Delay(_syncInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Investment Position Sync Service is stopping");
                break;
            }
        }
    }

    private async Task SyncPositionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var investmentRepository = scope.ServiceProvider.GetRequiredService<IInvestmentRepository>();
        var connectionRepository = scope.ServiceProvider.GetRequiredService<IExchangeConnectionRepository>();
        var whiteBitClient = scope.ServiceProvider.GetRequiredService<IWhiteBitApiClient>();
        var rewardCalculation = scope.ServiceProvider.GetRequiredService<IRewardCalculationService>();
        var encryptionService = scope.ServiceProvider.GetRequiredService<IExchangeCredentialEncryptionService>();

        var activePositions = await investmentRepository.GetActivePositionsAsync();

        if (!activePositions.Any())
        {
            _logger.LogDebug("No active positions to sync");
            return;
        }

        _logger.LogInformation("Starting sync for {Count} active positions", activePositions.Count);

        var syncedCount = 0;
        var errorCount = 0;

        foreach (var position in activePositions)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Sync cancelled during position processing");
                break;
            }

            try
            {
                await SyncSinglePositionAsync(
                    position,
                    connectionRepository,
                    whiteBitClient,
                    rewardCalculation,
                    encryptionService,
                    investmentRepository);

                syncedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync position {PositionId}", position.Id);
                errorCount++;
            }
        }

        _logger.LogInformation(
            "Position sync completed: {Synced} synced, {Errors} errors",
            syncedCount, errorCount);
    }

    private async Task SyncSinglePositionAsync(
        InvestmentPosition position,
        IExchangeConnectionRepository connectionRepository,
        IWhiteBitApiClient whiteBitClient,
        IRewardCalculationService rewardCalculation,
        IExchangeCredentialEncryptionService encryptionService,
        IInvestmentRepository investmentRepository)
    {
        // Get exchange connection
        var connection = await connectionRepository.GetByIdAsync(position.ExchangeConnectionId);
        if (connection == null)
        {
            _logger.LogWarning("Connection not found for position {PositionId}", position.Id);
            return;
        }

        if (!connection.IsActive)
        {
            _logger.LogWarning("Connection inactive for position {PositionId}", position.Id);
            return;
        }

        // Decrypt credentials
        string apiKey, apiSecret;
        try
        {
            apiKey = await encryptionService.DecryptAsync(connection.ApiKeyEncrypted, position.UserId);
            apiSecret = await encryptionService.DecryptAsync(connection.ApiSecretEncrypted, position.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt credentials for position {PositionId}", position.Id);
            return;
        }

        // Calculate accrued rewards locally
        var startDate = position.StartDate ?? position.CreatedAt;
        var accruedRewards = rewardCalculation.CalculateAccruedReward(
            position.PrincipalAmount,
            position.Apy,
            startDate);

        var currentValue = rewardCalculation.CalculateCurrentValue(
            position.PrincipalAmount,
            accruedRewards);

        // Update position
        var hasChanges = false;

        if (position.AccruedRewards != accruedRewards)
        {
            position.AccruedRewards = accruedRewards;
            hasChanges = true;
        }

        if (position.CurrentValue != currentValue)
        {
            position.CurrentValue = currentValue;
            hasChanges = true;
        }

        if (hasChanges)
        {
            position.LastSyncedAt = DateTime.UtcNow;
            await investmentRepository.UpdateAsync(position);

            _logger.LogDebug(
                "Updated position {PositionId}: CurrentValue={CurrentValue:F8}, AccruedRewards={AccruedRewards:F8}",
                position.Id, currentValue, accruedRewards);
        }

        // Optional: Verify with WhiteBit API (may have rate limits)
        // For MVP, we're calculating rewards locally
        // In production, you may want to periodically verify with exchange
        /*
        try
        {
            var whiteBitResponse = await whiteBitClient.GetInvestmentStatusAsync(
                apiKey, apiSecret, position.ExternalPositionId);

            if (whiteBitResponse.Status == "closed")
            {
                position.Status = InvestmentStatus.Closed;
                position.EndDate = DateTime.UtcNow;
                await investmentRepository.UpdateAsync(position);
            }
        }
        catch (Exception ex)
        {
            // Don't fail the entire sync if WhiteBit API call fails
            _logger.LogWarning(ex, "Failed to verify position {PositionId} with WhiteBit", position.Id);
        }
        */
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Investment Position Sync Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}
