using CashControl.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashControl.Infrastructure.Data.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(category => category.Id);

        builder.HasQueryFilter(category => category.IsActive);

        builder
            .Property(category => category.Id)
            .HasConversion(id => id.Value, value => CategoryId.Create(value));

        builder
            .Property(category => category.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();

        builder.Property(category => category.LastUpdate).IsRequired(false);

        builder.Property(category => category.IsActive).HasDefaultValue(true).IsRequired();

        builder.Property(category => category.Name).IsRequired().HasMaxLength(200);
    }
}
