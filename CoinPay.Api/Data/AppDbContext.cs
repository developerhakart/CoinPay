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
