using CashControl.Domain.Categories;
using CashControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashControl.IntegrationTests.Extensions;

public static class CategoryDbContextExtensions
{
    public static async Task<Category?> GetCategoryByIdAsync(
        this CashControlDbContext context,
        Guid id
    )
    {
        CategoryId categoryId = CategoryId.Create(id);
        Category? categoryInDb = await context
            .Categories.AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == categoryId);
        return categoryInDb;
    }

    public static async Task<Guid> CreateCategoryAsync(
        this CashControlDbContext context,
        string name
    )
    {
        Category category = Category.Create(name);
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category.Id.Value;
    }
}
