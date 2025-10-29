using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoinPay.Api.Data.Configurations;

public class PayoutAuditLogConfiguration : IEntityTypeConfiguration<PayoutAuditLog>
{
    public void Configure(EntityTypeBuilder<PayoutAuditLog> builder)
    {
        builder.ToTable("PayoutAuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.PayoutTransactionId)
            .IsRequired();

        builder.Property(a => a.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.PreviousStatus)
            .HasMaxLength(50);

        builder.Property(a => a.NewStatus)
            .HasMaxLength(50);

        builder.Property(a => a.EventData)
            .HasColumnType("jsonb"); // PostgreSQL JSONB for better performance

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(a => a.PayoutTransactionId)
            .HasDatabaseName("IX_PayoutAuditLogs_PayoutTransactionId");

        builder.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("IX_PayoutAuditLogs_CreatedAt");

        // Relationships
        builder.HasOne(a => a.PayoutTransaction)
            .WithMany(p => p.AuditLogs)
            .HasForeignKey(a => a.PayoutTransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
