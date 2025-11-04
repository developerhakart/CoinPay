using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoinPay.Api.Data.Configurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("BankAccounts");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedOnAdd();

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.AccountHolderName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.RoutingNumberEncrypted)
            .IsRequired();

        builder.Property(b => b.AccountNumberEncrypted)
            .IsRequired();

        builder.Property(b => b.LastFourDigits)
            .IsRequired()
            .HasMaxLength(4);

        builder.Property(b => b.AccountType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.BankName)
            .HasMaxLength(255);

        builder.Property(b => b.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(b => b.IsVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .IsRequired();

        builder.Property(b => b.DeletedAt);

        // Indexes
        builder.HasIndex(b => b.UserId)
            .HasDatabaseName("IX_BankAccounts_UserId");

        builder.HasIndex(b => new { b.UserId, b.IsPrimary })
            .HasDatabaseName("IX_BankAccounts_UserId_IsPrimary");

        // Relationships
        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.PayoutTransactions)
            .WithOne(p => p.BankAccount)
            .HasForeignKey(p => p.BankAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
