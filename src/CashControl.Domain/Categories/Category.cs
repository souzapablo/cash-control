using CashControl.Domain.Primitives;

namespace CashControl.Domain.Categories;

public class Category : Entity<CategoryId>
{
    protected Category() { }

    private Category(string name)
    {
        Id = CategoryId.CreateNew();
        Name = name;
    }

    public string Name { get; private set; } = string.Empty;

    public static Category Create(string name) => new(name);
}
