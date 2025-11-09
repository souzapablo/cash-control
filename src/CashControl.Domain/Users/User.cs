using CashControl.Domain.Primitives;

namespace CashControl.Domain.Users;

public class User : Entity<UserId>
{
    protected User() { }

    private User(Email email, string passwordHash)
    {
        Email = email;
        PasswordHash = passwordHash;
    }

    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
}
