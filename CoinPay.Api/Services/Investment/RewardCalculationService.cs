namespace CoinPay.Api.Services.Investment;

/// <summary>
/// Service for accurate financial calculations with 8-decimal precision
/// </summary>
public class RewardCalculationService : IRewardCalculationService
{
    private readonly ILogger<RewardCalculationService> _logger;

    public RewardCalculationService(ILogger<RewardCalculationService> logger)
    {
        _logger = logger;
    }

    public decimal CalculateDailyReward(decimal principal, decimal apy)
    {
        // Formula: Daily Reward = Principal × (APY / 365 / 100)
        // Example: 500 USDC @ 8.5% APY = 500 × (8.5 / 365 / 100) = 0.11643836 USDC/day

        if (principal <= 0)
            throw new ArgumentException("Principal must be positive", nameof(principal));

        if (apy < 0)
            throw new ArgumentException("APY cannot be negative", nameof(apy));

        var dailyReward = principal * (apy / 365m / 100m);

        // Round to 8 decimal places for precision
        return Math.Round(dailyReward, 8);
    }

    public decimal CalculateAccruedReward(decimal principal, decimal apy, DateTime startDate, DateTime? endDate = null)
    {
        var end = endDate ?? DateTime.UtcNow;
        var timeSpan = end - startDate;

        // Handle partial days with decimal days
        var days = (decimal)timeSpan.TotalDays;

        if (days < 0)
            throw new ArgumentException("End date must be after start date");

        var dailyReward = CalculateDailyReward(principal, apy);
        var accruedReward = dailyReward * days;

        _logger.LogDebug(
            "Calculated accrued reward: Principal={Principal}, APY={APY}, Days={Days:F6}, DailyReward={DailyReward:F8}, AccruedReward={AccruedReward:F8}",
            principal, apy, days, dailyReward, accruedReward);

        return Math.Round(accruedReward, 8);
    }

    public decimal CalculateProjectedReward(decimal principal, decimal apy, int days)
    {
        if (days < 0)
            throw new ArgumentException("Days cannot be negative", nameof(days));

        var dailyReward = CalculateDailyReward(principal, apy);
        var projectedReward = dailyReward * days;

        return Math.Round(projectedReward, 8);
    }

    public int CalculateDaysHeld(DateTime startDate, DateTime? endDate = null)
    {
        var end = endDate ?? DateTime.UtcNow;
        var timeSpan = end - startDate;

        return (int)Math.Floor(timeSpan.TotalDays);
    }

    public decimal CalculateCurrentValue(decimal principal, decimal accruedRewards)
    {
        return Math.Round(principal + accruedRewards, 8);
    }
}
