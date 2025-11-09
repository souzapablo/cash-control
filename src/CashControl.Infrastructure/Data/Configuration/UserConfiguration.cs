using CashControl.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashControl.Infrastructure.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);

        builder.HasQueryFilter(user => user.IsActive);

        builder
            .Property(user => user.Id)
            .HasConversion(id => id.Value, value => UserId.Create(value));

        builder
            .Property(user => user.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();

        builder.Property(user => user.LastUpdate).IsRequired(false);

        builder.Property(user => user.IsActive).HasDefaultValue(true).IsRequired();

        builder.Property(user => user.PasswordHash).IsRequired().HasMaxLength(100);

        builder.OwnsOne(
            user => user.Email,
            emailBuilder =>
            {
                emailBuilder
                    .Property(email => email.Address)
                    .HasColumnName("email")
                    .IsRequired()
                    .HasMaxLength(254);

                emailBuilder.HasIndex(email => email.Address).IsUnique();
            }
        );
    }
}
