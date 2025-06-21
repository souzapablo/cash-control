using CashControl.API.Abstractions;

namespace CashControl.API.Domain.Entities;

public class User(string email, string username, string firstName, string lastName) : Entity
{
    public string Username { get; private set; } = username;
    public string Email { get; private set; } = email;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
}