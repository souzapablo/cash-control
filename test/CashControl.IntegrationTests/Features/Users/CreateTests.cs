using System.Net;
using System.Net.Http.Json;
using CashControl.Domain.Errors;
using CashControl.Domain.Users;
using CashControl.IntegrationTests.Extensions;
using CashControl.IntegrationTests.Infrastructure;
using CashControl.IntegrationTests.Models;
using CashControl.IntegrationTests.Models.Users;
using Xunit;
using static CashControl.Api.Feature.Users.Create;

namespace CashControl.IntegrationTests.Features.Users;

public class CreateTests : BaseIntegrationTest
{
    public CreateTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact(DisplayName = "Should return 201 Created when user is created successfully")]
    public async Task Should_ReturnCreatedStatusCode_When_UserIsCreated()
    {
        // Arrange
        Command command = new("201created@example.com", "Password@123");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact(DisplayName = "Should return location header with user id when user is created")]
    public async Task Should_ReturnLocationHeader_When_UserIsCreated()
    {
        // Arrange
        Command command = new("location@example.com", "Test123!@#");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        Assert.NotNull(response.Headers.Location);

        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        var location = response.Headers.Location.ToString();
        Assert.Contains(result!.Value.Id.ToString(), location);
    }

    [Fact(DisplayName = "Should persist user to database with correct email")]
    public async Task Should_PersistUserToDatabase_When_UserIsCreated()
    {
        // Arrange
        var email = "persisted@example.com";
        Command command = new(email, "Test123!@#");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        User? userInDb = await Context.GetUserByIdAsync(result!.Value.Id);

        Assert.NotNull(userInDb);
        Assert.Equal(email, userInDb.Email.Address);
    }

    [Fact(DisplayName = "Should hash password when user is created")]
    public async Task Should_HashPassword_When_UserIsCreated()
    {
        // Arrange
        var password = "Test123!@#";
        Command command = new("password@example.com", password);

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        User? userInDb = await Context.GetUserByIdAsync(result!.Value.Id);

        Assert.NotNull(userInDb);
        Assert.NotEqual(password, userInDb.PasswordHash);
    }

    [Fact(DisplayName = "Should normalize email to lowercase when user is created")]
    public async Task Should_NormalizeEmailToLowercase_When_UserIsCreated()
    {
        // Arrange
        Command command = new("UPPERCASE@EXAMPLE.COM", "Test123!@#");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        User? userInDb = await Context.GetUserByIdAsync(result!.Value.Id);

        Assert.NotNull(userInDb);
        Assert.Equal("uppercase@example.com", userInDb.Email.Address);
    }

    [Theory(DisplayName = "Should create user with valid password formats")]
    [InlineData("Test123!@#")]
    [InlineData("Password1!")]
    [InlineData("MyP@ssw0rd")]
    [InlineData("Str0ng#Pass")]
    [InlineData("A1b2C3d4!@#$%^&*")]
    public async Task Should_CreateUser_When_PasswordIsValid(string password)
    {
        // Arrange
        var email = $"test{Guid.NewGuid()}@example.com";
        Command command = new(email, password);

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact(DisplayName = "Should return 400 Bad Request when email is null")]
    public async Task Should_ReturnBadRequest_When_EmailIsNull()
    {
        // Arrange
        Command command = new(null!, "Test123!@#");

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Email must be informed."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when email is empty string")]
    public async Task Should_ReturnBadRequest_When_EmailIsEmpty()
    {
        // Arrange
        Command command = new(string.Empty, "Test123!@#");

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Email must be informed."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when email is whitespace")]
    public async Task Should_ReturnBadRequest_When_EmailIsWhitespace()
    {
        // Arrange
        Command command = new("   ", "Test123!@#");

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Email must be informed."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when password is null")]
    public async Task Should_ReturnBadRequest_When_PasswordIsNull()
    {
        // Arrange
        Command command = new("test@example.com", null!);

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Password must be informed."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when password is empty string")]
    public async Task Should_ReturnBadRequest_When_PasswordIsEmpty()
    {
        // Arrange
        Command command = new("test@example.com", string.Empty);

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Password must be informed."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when password is whitespace")]
    public async Task Should_ReturnBadRequest_When_PasswordIsWhitespace()
    {
        // Arrange
        Command command = new("test@example.com", "   ");

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("The field Password must be informed."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when password is too short")]
    public async Task Should_ReturnBadRequest_When_PasswordIsTooShort()
    {
        // Arrange
        Command command = new("test@example.com", "Test1!@");

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError(
                "Password must be strong (8 to 64 chars, upper, lower, number, special)."
            ),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when password is too long")]
    public async Task Should_ReturnBadRequest_When_PasswordIsTooLong()
    {
        // Arrange
        var tooLongPassword = "Test123!@" + new string('A', 64);
        Command command = new("test@example.com", tooLongPassword);

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError(
                "Password must be strong (8 to 64 chars, upper, lower, number, special)."
            ),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when password lacks uppercase letter")]
    public async Task Should_ReturnBadRequest_When_PasswordLacksUppercase()
    {
        // Arrange
        Command command = new("test@example.com", "test123!@#");

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError(
                "Password must be strong (8 to 64 chars, upper, lower, number, special)."
            ),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when password lacks lowercase letter")]
    public async Task Should_ReturnBadRequest_When_PasswordLacksLowercase()
    {
        // Arrange
        Command command = new("test@example.com", "TEST123!@#");

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError(
                "Password must be strong (8 to 64 chars, upper, lower, number, special)."
            ),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when password lacks number")]
    public async Task Should_ReturnBadRequest_When_PasswordLacksNumber()
    {
        // Arrange
        Command command = new("test@example.com", "TestPass!@#");

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError(
                "Password must be strong (8 to 64 chars, upper, lower, number, special)."
            ),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when password lacks special character")]
    public async Task Should_ReturnBadRequest_When_PasswordLacksSpecialCharacter()
    {
        // Arrange
        Command command = new("test@example.com", "TestPass123");

        // Act
        var response = await Client.PostAsJsonAsync("/api/users", command);

        // Assert
        var result = await response.ReadAsResultAsync<CreateUserResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError(
                "Password must be strong (8 to 64 chars, upper, lower, number, special)."
            ),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when email already exists")]
    public async Task Should_ReturnBadRequest_When_EmailAlreadyExists()
    {
        // Arrange
        Command duplicateCommand = new(Data.DefaultUser.Email.Address, "Test123!@#");

        // Act
        var duplicateResponse = await Client.PostAsJsonAsync("/api/users", duplicateCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact(
        DisplayName = "Should return 400 Bad Request when email already exists (case insensitive)"
    )]
    public async Task Should_ReturnBadRequest_When_EmailAlreadyExists_CaseInsensitive()
    {
        // Arrange
        Command duplicateCommand = new(Data.DefaultUser.Email.Address.ToUpper(), "Test123!@#");

        // Act
        var duplicateResponse = await Client.PostAsJsonAsync("/api/users", duplicateCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact(DisplayName = "Should return EmailAlreadyExists error when email already exists")]
    public async Task Should_ReturnEmailAlreadyExists_When_EmailAlreadyExists()
    {
        // Arrange
        Command duplicateCommand = new(Data.DefaultUser.Email.Address, "Test123!@#");

        // Act
        var duplicateResponse = await Client.PostAsJsonAsync("/api/users", duplicateCommand);

        // Assert
        var result = await duplicateResponse.ReadAsResultAsync<Result>();
        Assert.Equal(UserErrors.EmailAlreadyExists, result?.Error);
    }

    [Fact(
        DisplayName = "Should return EmailAlreadyExists error when email already exists (case insensitive)"
    )]
    public async Task Should_ReturnEmailAlreadyExists_When_EmailAlreadyExists_CaseInsensitive()
    {
        // Arrange
        Command duplicateCommand = new(Data.DefaultUser.Email.Address.ToUpper(), "Test123!@#");

        // Act
        var duplicateResponse = await Client.PostAsJsonAsync("/api/users", duplicateCommand);

        // Assert
        var result = await duplicateResponse.ReadAsResultAsync<Result>();
        Assert.Equal(UserErrors.EmailAlreadyExists, result?.Error);
    }
}
