namespace CoinPay.Api.DTOs.Exchange;

public class WhiteBitBalanceResponse
{
    public Dictionary<string, decimal> Balances { get; set; } = new();
}

public class WhiteBitPlansResponse
{
    public List<WhiteBitPlan> Plans { get; set; } = new();
}

public class WhiteBitPlan
{
    public string PlanId { get; set; } = string.Empty;
    public string Asset { get; set; } = string.Empty;
    public decimal Apy { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public string Term { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class WhiteBitInvestmentsResponse
{
    public List<WhiteBitInvestment> Investments { get; set; } = new();
}

public class WhiteBitInvestment
{
    public string InvestmentId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Asset { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class WhiteBitCreateInvestmentResponse
{
    public string InvestmentId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Asset { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class WhiteBitCloseInvestmentResponse
{
    public string InvestmentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal FinalAmount { get; set; }
    public DateTime ClosedAt { get; set; }
}

public class WhiteBitDepositAddressResponse
{
    public string Asset { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Network { get; set; } = string.Empty;
    public string? Memo { get; set; }
}

// Request/Response DTOs for our API
public class ConnectWhiteBitRequest
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}

public class ConnectWhiteBitResponse
{
    public Guid ConnectionId { get; set; }
    public string ExchangeName { get; set; } = "whitebit";
    public string Status { get; set; } = "active";
    public DateTime ConnectedAt { get; set; }
}

public class ExchangeConnectionStatusResponse
{
    public bool Connected { get; set; }
    public Guid? ConnectionId { get; set; }
    public string? ExchangeName { get; set; }
    public DateTime? ConnectedAt { get; set; }
    public DateTime? LastValidated { get; set; }
}

public class InvestmentPlanResponse
{
    public string PlanId { get; set; } = string.Empty;
    public string Asset { get; set; } = string.Empty;
    public decimal Apy { get; set; }
    public string ApyFormatted { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public string Term { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateInvestmentRequest
{
    public string PlanId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Asset { get; set; } = "USDC"; // Default to USDC, but can be DUSDT or DBTC for demo tokens
    public Guid WalletId { get; set; }
}

public class CreateInvestmentResponse
{
    public Guid InvestmentId { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string Asset { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Apy { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal EstimatedDailyReward { get; set; }
    public decimal EstimatedMonthlyReward { get; set; }
    public decimal EstimatedYearlyReward { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class InvestmentPositionResponse
{
    public Guid Id { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string Asset { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal AccruedRewards { get; set; }
    public decimal Apy { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public int DaysHeld { get; set; }
    public decimal EstimatedDailyReward { get; set; }
    public decimal EstimatedMonthlyReward { get; set; }
    public decimal EstimatedYearlyReward { get; set; }
}

public class InvestmentPositionDetailResponse : InvestmentPositionResponse
{
    public string PlanName { get; set; } = string.Empty;
    public DateTime? EndDate { get; set; }
    public List<InvestmentTransactionResponse> Transactions { get; set; } = new();
    public ProjectedRewardsResponse ProjectedRewards { get; set; } = new();
}

public class ProjectedRewardsResponse
{
    public decimal Daily { get; set; }
    public decimal Monthly { get; set; }
    public decimal Yearly { get; set; }
}

public class InvestmentTransactionResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class WithdrawInvestmentRequest
{
    public Guid WalletId { get; set; }
}

public class WithdrawInvestmentResponse
{
    public Guid InvestmentId { get; set; }
    public decimal WithdrawalAmount { get; set; }
    public decimal Principal { get; set; }
    public decimal Rewards { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime EstimatedCompletionTime { get; set; }
}
