using CashControl.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashControl.API.Data.Persistence.Mappings;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasMaxLength(128);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Username).HasMaxLength(40);
        builder.HasIndex(u => u.Username).IsUnique();
        builder.Property(u => u.FirstName).HasMaxLength(60);
        builder.Property(u => u.LastName).HasMaxLength(60);
    }
}