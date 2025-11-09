using CashControl.Domain.Primitives;

namespace CashControl.Domain.Categories;

public class CategoryId : EntityId<Guid>
{
    protected CategoryId() { }

    private CategoryId(Guid value)
        : base(value) { }

    public static CategoryId Create(Guid value) => new(value);

    public static CategoryId CreateNew() => new(Guid.NewGuid());
}
