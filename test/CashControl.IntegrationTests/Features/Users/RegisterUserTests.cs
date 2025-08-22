using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CashControl.App.Features.Users;
using CashControl.IntegrationTests.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CashControl.IntegrationTests.Features.Users;

[Collection("Register user")]
public class RegisterUserEndpointTests : BaseIntegrationTest
{
    private const string ApiRoute = "api/v1/users";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RegisterUserEndpointTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact(DisplayName = "Should return 201 Created with user ID when request is valid")]
    public async Task ShouldReturnCreatedWithUserId_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterUserCommand("newuser", "newuser@email.com", "password");

        // Act
        var response = await _client.PostAsJsonAsync(ApiRoute, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Result<RegisterUserResponse>>(_jsonOptions);

        Assert.NotNull(result?.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id);

        var expectedLocation = $"{ApiRoute}/{result.Value.Id}";
        Assert.Equal(expectedLocation, response.Headers.Location?.ToString());
    }

    [Fact(DisplayName = "Should persist user in database when request is valid")]
    public async Task ShouldPersistUserInDatabase_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterUserCommand("dbuser", "dbuser@email.com", "password");

        // Act
        var response = await _client.PostAsJsonAsync(ApiRoute, request);

        var result = await response.Content.ReadFromJsonAsync<Result<RegisterUserResponse>>(_jsonOptions);

        // Assert 
        var userId = UserId.From(result!.Value!.Id);
        var createdUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        Assert.NotNull(createdUser);
        Assert.Equal(request.Email, createdUser.Email);
        Assert.Equal(request.Username, createdUser.Username);
    }
    

    [Fact(DisplayName = "Should return 400 Bad Request when email is already registered")]
    public async Task ShouldReturnBadRequest_WhenEmailIsAlreadyRegistered()
    {
        // Arrange
        var request = new RegisterUserCommand("duplicated", "duplicated@email.com", "password");

        // Act
        await _client.PostAsJsonAsync(ApiRoute, request);
        var response = await _client.PostAsJsonAsync(ApiRoute, request);
        var rawContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Result<RegisterUserResponse>>(rawContent, _jsonOptions);

        // Assert
        Assert.False(result!.IsSuccess);
        Assert.Equal(UserErrors.EmailAlreadyRegistered, result.Error);
    }
}
