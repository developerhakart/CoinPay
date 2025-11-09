namespace CoinPay.Api.Models;

public enum InvestmentStatus
{
    Pending,
    Active,
    Closed,
    Failed
}

public class InvestmentPosition
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int UserId1 { get; set; } // EF Core shadow property - actual FK to Users.Id
    public Guid? ExchangeConnectionId { get; set; } // Nullable for demo token investments
    public string ExchangeName { get; set; } = "whitebit";
    public string? ExternalPositionId { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string Asset { get; set; } = "USDC";
    public decimal PrincipalAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal AccruedRewards { get; set; }
    public decimal Apy { get; set; }
    public InvestmentStatus Status { get; set; } = InvestmentStatus.Pending;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
    public ExchangeConnection? ExchangeConnection { get; set; } // Nullable for demo token investments
    public ICollection<InvestmentTransaction> Transactions { get; set; } = new List<InvestmentTransaction>();
}

public enum InvestmentTransactionType
{
    Create,
    Deposit,
    Withdraw,
    Reward
}

public enum InvestmentTransactionStatus
{
    Pending,
    Confirmed,
    Failed
}

public class InvestmentTransaction
{
    public Guid Id { get; set; }
    public Guid InvestmentPositionId { get; set; }
    public Guid UserId { get; set; }
    public int UserId1 { get; set; } // EF Core shadow property - actual FK to Users.Id
    public InvestmentTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Asset { get; set; } = "USDC";
    public string? ExternalTransactionId { get; set; }
    public InvestmentTransactionStatus Status { get; set; } = InvestmentTransactionStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public InvestmentPosition InvestmentPosition { get; set; } = null!;
    public User User { get; set; } = null!;
}
