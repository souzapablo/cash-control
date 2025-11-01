using CashControl.Domain.Accounts;
using CashControl.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashControl.Infrastructure.Data.Configuration;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(account => account.Id);

        builder.Property(account => account.Id)
            .HasConversion(
                    id => id.Value,
                    value => AccountId.Create(value)
                );
        
        builder.Property(account => account.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();

        builder.Property(account => account.LastUpdate)
            .IsRequired(false);

        builder.Property(account => account.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.OwnsOne(account => account.Balance, balanceBuilder =>
        {
            balanceBuilder.Property(money => money.Value)
                .HasColumnName("balance_amount")
                .HasPrecision(18, 4)
                .HasDefaultValue(0m)
                .IsRequired();
            
            balanceBuilder.Property(money => money.Currency)
                .HasColumnName("balance_currency")
                .HasDefaultValue(Currency.BRL)
                .IsRequired();
        });
        
        builder.Property(account => account.Name)
            .IsRequired()
            .HasMaxLength(200);
    }
}