namespace CoinPay.Api.Services.Investment;

/// <summary>
/// Service for calculating investment rewards and projections
/// </summary>
public interface IRewardCalculationService
{
    /// <summary>
    /// Calculate daily reward based on principal and APY
    /// </summary>
    decimal CalculateDailyReward(decimal principal, decimal apy);

    /// <summary>
    /// Calculate accrued rewards from start date to now
    /// </summary>
    decimal CalculateAccruedReward(decimal principal, decimal apy, DateTime startDate, DateTime? endDate = null);

    /// <summary>
    /// Calculate projected reward for specified number of days
    /// </summary>
    decimal CalculateProjectedReward(decimal principal, decimal apy, int days);

    /// <summary>
    /// Calculate number of days held
    /// </summary>
    int CalculateDaysHeld(DateTime startDate, DateTime? endDate = null);

    /// <summary>
    /// Calculate current value (principal + accrued rewards)
    /// </summary>
    decimal CalculateCurrentValue(decimal principal, decimal accruedRewards);
}
