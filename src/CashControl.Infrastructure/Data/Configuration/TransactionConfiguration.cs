using CashControl.Domain.Enums;
using CashControl.Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashControl.Infrastructure.Data.Configuration;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(transaction => transaction.Id);

        builder.HasQueryFilter(transaction => transaction.IsActive);

        builder
            .Property(transaction => transaction.Id)
            .HasConversion(id => id.Value, value => TransactionId.Create(value));

        builder
            .Property(account => account.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();

        builder.Property(transaction => transaction.LastUpdate).IsRequired(false);

        builder.Property(transaction => transaction.IsActive).HasDefaultValue(true).IsRequired();

        builder.OwnsOne(
            transaction => transaction.Amount,
            balanceBuilder =>
            {
                balanceBuilder
                    .Property(money => money.Value)
                    .HasColumnName("amount")
                    .HasPrecision(18, 4)
                    .HasDefaultValue(0m)
                    .IsRequired();

                balanceBuilder
                    .Property(money => money.Currency)
                    .HasColumnName("currency")
                    .HasDefaultValue(Currency.BRL)
                    .IsRequired();
            }
        );

        builder.Property(account => account.Description).IsRequired().HasMaxLength(200);

        builder.Property(transaction => transaction.Date).IsRequired();

        builder.Property(transaction => transaction.Type).IsRequired();

        builder
            .HasOne(transaction => transaction.Category)
            .WithMany()
            .HasForeignKey(transaction => transaction.CategoryId)
            .IsRequired();
    }
}
