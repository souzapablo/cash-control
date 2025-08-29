using CashControl.App.Abstractions;

namespace CashControl.App.Features.Users;

public class User : Entity<UserId>
{
    private User(UserId id, string username, string passwordHash, string email) : base(id)
    {
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
    }
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public static User Register(string username, string passwordHash, string email, Guid? id = null) =>
        new(id is not null ? UserId.From(id.Value) : UserId.New(), username, passwordHash, email);
}

public sealed class UserId(Guid value) : EntityId<UserId>(value) { }