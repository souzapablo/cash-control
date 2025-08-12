using CashControl.Api.Features.Users;

namespace CashControl.UnitTests.Builders;

public class UserBuilder
{
    private string _username = "test";
    private string _passwordHash = "test";
    private string _email = "test@email.com";

    public UserBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public User Build() => User.Register(_username, _passwordHash, _email);
}