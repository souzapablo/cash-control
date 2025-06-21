using System.ComponentModel.DataAnnotations;
using CashControl.API.Features.Users.Commands.Create;

namespace CashControl.API.Models.Users;

public class CreateUserInputModel(string email, string username, string firstName, string lastName)
{
    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    [MaxLength(128, ErrorMessage = "Email address cannot exceed 128 characters.")]
    public string Email { get; } = email;
    
    [MaxLength(40,  ErrorMessage = "Username cannot exceed 40 characters.")]
    public string Username { get; } = username;
    
    [MaxLength(60,  ErrorMessage = "First name cannot exceed 40 characters.")]
    public string FirstName { get; } = firstName;
    
    [MaxLength(60,  ErrorMessage = "Last name cannot exceed 60 characters.")]
    public string LastName { get; } = lastName;
}

public static class CreateUserInputModelExtensions
{
    public static CreateUserCommand ToCommand(this  CreateUserInputModel input)
    {
        return new CreateUserCommand(input.Email, input.Username, input.FirstName, input.LastName);
    }
}