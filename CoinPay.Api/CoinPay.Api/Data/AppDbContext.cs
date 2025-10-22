using Microsoft.EntityFrameworkCore;
using CoinPay.Api.Models;

namespace CoinPay.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed some initial data
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
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                CompletedAt = DateTime.UtcNow.AddDays(-2)
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
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                CompletedAt = DateTime.UtcNow.AddDays(-1)
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
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
