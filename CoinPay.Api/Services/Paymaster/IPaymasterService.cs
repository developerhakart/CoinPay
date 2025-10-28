using CoinPay.Api.Services.UserOperation;

namespace CoinPay.Api.Services.Paymaster;

/// <summary>
/// Service for Circle Paymaster integration (gas sponsorship)
/// </summary>
public interface IPaymasterService
{
    /// <summary>
    /// Get paymaster data for gas sponsorship
    /// </summary>
    Task<string> GetPaymasterDataAsync(UserOperationDto userOp, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify that a UserOperation will be sponsored
    /// </summary>
    Task<bool> VerifySponsorshipAsync(string userOpHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paymaster balance/status
    /// </summary>
    Task<PaymasterStatus> GetStatusAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Paymaster status information
/// </summary>
public class PaymasterStatus
{
    public string PaymasterAddress { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public int DailySponsorshipLimit { get; set; }
    public int DailySponsorshipUsed { get; set; }
}
