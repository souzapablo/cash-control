using System.Net;
using System.Net.Http.Json;
using CashControl.Domain.Errors;
using CashControl.IntegrationTests.Extensions;
using CashControl.IntegrationTests.Infrastructure;
using CashControl.IntegrationTests.Models;
using CashControl.IntegrationTests.Models.Auth;
using Xunit;
using static CashControl.Api.Feature.Auth.Login;

namespace CashControl.IntegrationTests.Features.Auth;

public class LoginTests : BaseIntegrationTest
{
    public LoginTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact(DisplayName = "Should return 200 OK when login is successful")]
    public async Task Should_ReturnOkStatusCode_When_LoginIsSuccessful()
    {
        // Arrange
        Command command = new(Data.DefaultUser.Email.Address, Data.DefaultPassword);

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "Should return access token when login is successful")]
    public async Task Should_ReturnAccessToken_When_LoginIsSuccessful()
    {
        // Arrange
        Command command = new(Data.DefaultUser.Email.Address, Data.DefaultPassword);

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        var result = await response.ReadAsResultAsync<LoginResponse>();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.AccessToken);
    }

    [Fact(DisplayName = "Should return 400 Bad Request when email is invalid")]
    public async Task Should_ReturnBadRequest_When_EmailIsInvalid()
    {
        // Arrange
        Command command = new("nonexistent@example.com", "DefaultPassword123!");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Should return InvalidCredentials error when email is invalid")]
    public async Task Should_ReturnInvalidCredentials_When_EmailIsInvalid()
    {
        // Arrange
        Command command = new("nonexistent@example.com", "DefaultPassword123!");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        var result = await response.ReadAsResultAsync<Result>();
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.InvalidCredentials, result.Error);
    }

    [Fact(DisplayName = "Should return 400 Bad Request when password is invalid")]
    public async Task Should_ReturnBadRequest_When_PasswordIsInvalid()
    {
        // Arrange
        Command command = new(Data.DefaultUser.Email.Address, "WrongPassword123!");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Should return InvalidCredentials error when password is invalid")]
    public async Task Should_ReturnInvalidCredentials_When_PasswordIsInvalid()
    {
        // Arrange
        Command command = new(Data.DefaultUser.Email.Address, "WrongPassword123!");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        var result = await response.ReadAsResultAsync<Result>();
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.InvalidCredentials, result.Error);
    }

    [Fact(DisplayName = "Should return 200 OK when email is case insensitive")]
    public async Task Should_ReturnOk_When_EmailIsCaseInsensitive()
    {
        // Arrange
        Command command = new(Data.DefaultUser.Email.Address.ToUpper(), Data.DefaultPassword);

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "Should return access token when email is case insensitive")]
    public async Task Should_ReturnAccessToken_When_EmailIsCaseInsensitive()
    {
        // Arrange
        Command command = new(Data.DefaultUser.Email.Address.ToUpper(), Data.DefaultPassword);

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/auth/login", command);

        // Assert
        var result = await response.ReadAsResultAsync<LoginResponse>();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.AccessToken);
    }
}
