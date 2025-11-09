using System.Net;
using System.Net.Http.Json;
using CashControl.Domain.Categories;
using CashControl.IntegrationTests.Extensions;
using CashControl.IntegrationTests.Infrastructure;
using CashControl.IntegrationTests.Models.Categories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static CashControl.Api.Feature.Categories.Create;

namespace CashControl.IntegrationTests.Features.Categories;

public class CreateTests : BaseIntegrationTest
{
    public CreateTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact(DisplayName = "Should return 201 Created when category is created successfully")]
    public async Task Should_ReturnCreatedStatusCode_When_CategoryIsCreated()
    {
        // Arrange
        Command command = new("My New Category");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/categories", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact(DisplayName = "Should return location header with category id when category is created")]
    public async Task Should_ReturnLocationHeader_When_CategoryIsCreated()
    {
        // Arrange
        Command command = new("Category with Location Header");

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/categories", command);

        // Assert
        Assert.NotNull(response.Headers.Location);

        var result = response.ReadAsResultAsync<CreateCategoryResponse>();
        var location = response.Headers.Location.ToString();
        Assert.Contains(result!.Value.Id.ToString(), location);
    }

    [Fact(DisplayName = "Should persist category to database with correct name")]
    public async Task Should_PersistCategoryToDatabase_When_CategoryIsCreated()
    {
        // Arrange
        var categoryName = "Persisted Category";
        Command command = new(categoryName);

        // Act
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/categories", command);

        // Assert
        var result = response.ReadAsResultAsync<CreateCategoryResponse>();
        Category? categoryInDb = await GetCategoryInDb(result?.Value.Id);

        Assert.NotNull(categoryInDb);
        Assert.Equal(categoryName, categoryInDb.Name);
    }

    [Fact(DisplayName = "Should create category with maximum length name (200 characters)")]
    public async Task Should_CreateCategory_When_NameIsMaximumLength()
    {
        // Arrange
        var maxLengthName = new string('A', 200);
        Command command = new(maxLengthName);

        // Act
        var response = await Client.PostAsJsonAsync("/api/categories", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = response.ReadAsResultAsync<CreateCategoryResponse>();
        Category? categoryInDb = await GetCategoryInDb(result?.Value.Id);

        Assert.NotNull(categoryInDb);
        Assert.Equal(maxLengthName, categoryInDb.Name);
    }

    [Fact(DisplayName = "Should create category with special characters in name")]
    public async Task Should_CreateCategory_When_NameContainsSpecialCharacters()
    {
        // Arrange
        Command command = new("Category #1 - $pecial & Symbols! @2024");

        // Act
        var response = await Client.PostAsJsonAsync("/api/categories", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = response.ReadAsResultAsync<CreateCategoryResponse>();
        Category? categoryInDb = await GetCategoryInDb(result?.Value.Id);

        Assert.NotNull(categoryInDb);
        Assert.Equal("Category #1 - $pecial & Symbols! @2024", categoryInDb.Name);
    }

    [Fact(DisplayName = "Should create category with unicode characters in name")]
    public async Task Should_CreateCategory_When_NameContainsUnicodeCharacters()
    {
        // Arrange
        Command command = new("Categoria Teste - 测试类别 - Тестова категорія");

        // Act
        var response = await Client.PostAsJsonAsync("/api/categories", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = response.ReadAsResultAsync<CreateCategoryResponse>();
        Category? categoryInDb = await GetCategoryInDb(result?.Value.Id);

        Assert.NotNull(categoryInDb);
        Assert.Equal("Categoria Teste - 测试类别 - Тестова категорія", categoryInDb.Name);
    }

    [Fact(DisplayName = "Should return 400 Bad Request when name exceeds maximum length")]
    public async Task Should_ReturnBadRequest_When_NameExceedsMaximumLength()
    {
        // Arrange
        var tooLongName = new string('A', 201);
        Command command = new(tooLongName);

        // Act
        var response = await Client.PostAsJsonAsync("/api/categories", command);

        // Assert
        var result = response.ReadAsResultAsync<CreateCategoryResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("Category name cannot exceed 200 characters."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when name is null")]
    public async Task Should_ReturnBadRequest_When_NameIsNull()
    {
        // Arrange
        Command command = new(null!);

        // Act
        var response = await Client.PostAsJsonAsync("/api/categories", command);

        // Assert
        var result = response.ReadAsResultAsync<CreateCategoryResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("Category name cannot be empty."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when name is empty string")]
    public async Task Should_ReturnBadRequest_When_NameIsEmpty()
    {
        // Arrange
        Command command = new(string.Empty);

        // Act
        var response = await Client.PostAsJsonAsync("/api/categories", command);

        // Assert
        var result = response.ReadAsResultAsync<CreateCategoryResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("Category name cannot be empty."),
            result?.Error
        );
    }

    [Fact(DisplayName = "Should return 400 Bad Request when name is whitespace")]
    public async Task Should_ReturnBadRequest_When_NameIsWhitespace()
    {
        // Arrange
        Command command = new("   ");

        // Act
        var response = await Client.PostAsJsonAsync("/api/categories", command);

        // Assert
        var result = response.ReadAsResultAsync<CreateCategoryResponse>();
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(
            Domain.Primitives.Error.ValidationError("Category name cannot be empty."),
            result?.Error
        );
    }

    private async Task<Category?> GetCategoryInDb(Guid? id)
    {
        if (id is null)
            return null;

        CategoryId categoryId = CategoryId.Create(id.Value);
        Category? categoryInDb = await Context
            .Categories.AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == categoryId);
        return categoryInDb;
    }
}
