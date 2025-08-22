using CashControl.App.Features.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashControl.App.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => UserId.From(value));

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(64)
            .IsRequired();
    }
}
