using Microsoft.EntityFrameworkCore;
using CoinPay.Api.Models;
using CoinPay.Api.Data.Configurations;

namespace CoinPay.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<BlockchainTransaction> BlockchainTransactions { get; set; }
    public DbSet<WebhookRegistration> WebhookRegistrations { get; set; }
    public DbSet<WebhookDeliveryLog> WebhookDeliveryLogs { get; set; }

    // Sprint N03: Phase 3 - Fiat Off-Ramp
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<PayoutTransaction> PayoutTransactions { get; set; }
    public DbSet<PayoutAuditLog> PayoutAuditLogs { get; set; }

    // Sprint N04: Phase 4 - Exchange Investment
    public DbSet<ExchangeConnection> ExchangeConnections { get; set; }
    public DbSet<InvestmentPosition> InvestmentPositions { get; set; }
    public DbSet<InvestmentTransaction> InvestmentTransactions { get; set; }

    // Sprint N05: Phase 5 - Basic Swap (DEX Integration)
    public DbSet<SwapTransaction> SwapTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations for Sprint N03 models
        modelBuilder.ApplyConfiguration(new BankAccountConfiguration());
        modelBuilder.ApplyConfiguration(new PayoutTransactionConfiguration());
        modelBuilder.ApplyConfiguration(new PayoutAuditLogConfiguration());

        // Configure BlockchainTransaction indexes
        modelBuilder.Entity<BlockchainTransaction>()
            .HasIndex(t => t.UserOpHash)
            .IsUnique();

        modelBuilder.Entity<BlockchainTransaction>()
            .HasIndex(t => t.TransactionHash);

        modelBuilder.Entity<BlockchainTransaction>()
            .HasIndex(t => t.WalletId);

        modelBuilder.Entity<BlockchainTransaction>()
            .HasIndex(t => t.Status);

        // Configure Wallet indexes
        modelBuilder.Entity<Wallet>()
            .HasIndex(w => w.Address)
            .IsUnique();

        modelBuilder.Entity<Wallet>()
            .HasIndex(w => w.UserId);

        // Configure WebhookRegistration indexes
        modelBuilder.Entity<WebhookRegistration>()
            .HasIndex(w => w.UserId);

        modelBuilder.Entity<WebhookRegistration>()
            .HasIndex(w => w.IsActive);

        // Configure WebhookDeliveryLog indexes
        modelBuilder.Entity<WebhookDeliveryLog>()
            .HasIndex(l => l.WebhookId);

        modelBuilder.Entity<WebhookDeliveryLog>()
            .HasIndex(l => l.TransactionId);

        modelBuilder.Entity<WebhookDeliveryLog>()
            .HasIndex(l => l.Timestamp);

        // Configure relationships
        modelBuilder.Entity<WebhookDeliveryLog>()
            .HasOne(l => l.Webhook)
            .WithMany(w => w.DeliveryLogs)
            .HasForeignKey(l => l.WebhookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WebhookDeliveryLog>()
            .HasOne(l => l.Transaction)
            .WithMany()
            .HasForeignKey(l => l.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure ExchangeConnection indexes (Sprint N04)
        modelBuilder.Entity<ExchangeConnection>()
            .HasIndex(e => e.UserId);

        modelBuilder.Entity<ExchangeConnection>()
            .HasIndex(e => new { e.UserId, e.ExchangeName })
            .IsUnique();

        modelBuilder.Entity<ExchangeConnection>()
            .HasIndex(e => e.IsActive);

        // Configure ExchangeConnection foreign key to Users (Sprint N04)
        modelBuilder.Entity<ExchangeConnection>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId1)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure InvestmentPosition indexes (Sprint N04)
        modelBuilder.Entity<InvestmentPosition>()
            .HasIndex(i => i.UserId);

        modelBuilder.Entity<InvestmentPosition>()
            .HasIndex(i => i.ExchangeConnectionId);

        modelBuilder.Entity<InvestmentPosition>()
            .HasIndex(i => i.Status);

        modelBuilder.Entity<InvestmentPosition>()
            .HasIndex(i => i.CreatedAt);

        // Configure InvestmentTransaction indexes (Sprint N04)
        modelBuilder.Entity<InvestmentTransaction>()
            .HasIndex(t => t.InvestmentPositionId);

        modelBuilder.Entity<InvestmentTransaction>()
            .HasIndex(t => t.UserId);

        modelBuilder.Entity<InvestmentTransaction>()
            .HasIndex(t => t.CreatedAt);

        // Configure InvestmentPosition relationships (Sprint N04)
        modelBuilder.Entity<InvestmentPosition>()
            .HasOne<ExchangeConnection>()
            .WithMany()
            .HasForeignKey(i => i.ExchangeConnectionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InvestmentPosition>()
            .HasMany(i => i.Transactions)
            .WithOne()
            .HasForeignKey(t => t.InvestmentPositionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure InvestmentPosition foreign key to Users (Sprint N04)
        modelBuilder.Entity<InvestmentPosition>()
            .HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(i => i.UserId1)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure InvestmentTransaction foreign key to Users (Sprint N04)
        modelBuilder.Entity<InvestmentTransaction>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId1)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure SwapTransaction indexes (Sprint N05)
        modelBuilder.Entity<SwapTransaction>()
            .HasIndex(s => s.UserId);

        modelBuilder.Entity<SwapTransaction>()
            .HasIndex(s => s.WalletAddress);

        modelBuilder.Entity<SwapTransaction>()
            .HasIndex(s => s.Status);

        modelBuilder.Entity<SwapTransaction>()
            .HasIndex(s => s.CreatedAt);

        modelBuilder.Entity<SwapTransaction>()
            .HasIndex(s => s.TransactionHash);

        // Sprint N06: Performance optimization indexes (BE-605)
        // Transaction status index for filtering by status
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.Status)
            .HasDatabaseName("IX_Transactions_Status");

        // Investment position indexes for user queries and status filtering
        modelBuilder.Entity<InvestmentPosition>()
            .HasIndex(i => new { i.UserId, i.Status })
            .HasDatabaseName("IX_InvestmentPositions_UserId_Status");

        // Swap transaction indexes for user history and date-based queries
        modelBuilder.Entity<SwapTransaction>()
            .HasIndex(s => new { s.UserId, s.CreatedAt })
            .HasDatabaseName("IX_SwapTransactions_UserId_CreatedAt");

        // Seed some initial data with static timestamps
        modelBuilder.Entity<Transaction>().HasData(
            new Transaction
            {
                Id = 1,
                TransactionId = "TXN001",
                Amount = 100.50m,
                Currency = "USD",
                Type = "Payment",
                Status = "Completed",
                SenderName = "John Doe",
                ReceiverName = "Jane Smith",
                Description = "Payment for services",
                CreatedAt = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc),
                CompletedAt = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 2,
                TransactionId = "TXN002",
                Amount = 250.00m,
                Currency = "USD",
                Type = "Transfer",
                Status = "Completed",
                SenderName = "Alice Johnson",
                ReceiverName = "Bob Wilson",
                Description = "Money transfer",
                CreatedAt = new DateTime(2025, 10, 26, 0, 0, 0, DateTimeKind.Utc),
                CompletedAt = new DateTime(2025, 10, 26, 0, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 3,
                TransactionId = "TXN003",
                Amount = 75.25m,
                Currency = "USD",
                Type = "Payment",
                Status = "Pending",
                SenderName = "Charlie Brown",
                ReceiverName = "David Lee",
                Description = "Pending payment",
                CreatedAt = new DateTime(2025, 10, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
