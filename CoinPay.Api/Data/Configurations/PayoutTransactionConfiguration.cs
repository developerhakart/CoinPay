using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoinPay.Api.Data.Configurations;

public class PayoutTransactionConfiguration : IEntityTypeConfiguration<PayoutTransaction>
{
    public void Configure(EntityTypeBuilder<PayoutTransaction> builder)
    {
        builder.ToTable("PayoutTransactions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.BankAccountId)
            .IsRequired();

        builder.Property(p => p.GatewayTransactionId)
            .HasMaxLength(255);

        builder.Property(p => p.UsdcAmount)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(p => p.UsdAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.ExchangeRate)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(p => p.ConversionFee)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.PayoutFee)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.TotalFees)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.NetAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("pending");

        builder.Property(p => p.FailureReason);

        builder.Property(p => p.InitiatedAt)
            .IsRequired();

        builder.Property(p => p.CompletedAt);

        builder.Property(p => p.EstimatedArrival);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_PayoutTransactions_UserId");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_PayoutTransactions_Status");

        builder.HasIndex(p => p.GatewayTransactionId)
            .HasDatabaseName("IX_PayoutTransactions_GatewayTransactionId");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_PayoutTransactions_CreatedAt");

        // Relationships
        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.BankAccount)
            .WithMany(b => b.PayoutTransactions)
            .HasForeignKey(p => p.BankAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.AuditLogs)
            .WithOne(a => a.PayoutTransaction)
            .HasForeignKey(a => a.PayoutTransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
