using CashControl.Domain.Users;
using Xunit;

namespace CashControl.UnitTests.Entities.Users;

public class EmailTests
{
    [Fact(DisplayName = "Should create email with valid address")]
    public void Should_CreateEmail_When_ValidAddress()
    {
        // Arrange
        var address = "user@example.com";

        // Act
        var email = Email.Create(address);

        // Assert
        Assert.Equal(address, email.Address);
    }

    [Theory(DisplayName = "Emails with same address should be equal")]
    [InlineData("user@example.com", "user@example.com")]
    [InlineData("user@example.com", "user@exampLe.com")]
    [InlineData("user@example.com", "user@exampLe.com ")]
    [InlineData("user@example.com", " user@exampLe.com")]
    [InlineData("user@example.com", " user@exampLe.com ")]
    public void Should_BeEqual_When_SameAddress(string address, string address2)
    {
        // Act
        var email = Email.Create(address);
        var email2 = Email.Create(address2);

        // Assert
        Assert.Equal(email, email2);
    }

    [Fact(DisplayName = "Emails with same different address should not be equal")]
    public void Should_NotBeEqual_When_DifferentAddress()
    {
        //Arrange
        var address = "user@example.com";
        var address2 = "different@example.com";

        // Act
        var email = Email.Create(address);
        var email2 = Email.Create(address2);

        // Assert
        Assert.NotEqual(email, email2);
    }

    [Theory(DisplayName = "Should throw ArgumentException when email address is invalid")]
    [InlineData("email")]
    [InlineData("user@.com")]
    [InlineData("user@com")]
    [InlineData("@example.com")]
    [InlineData("user@example..com")]
    [InlineData(".user@example.com")]
    public void Should_ThrowArgumentException_When_EmailAddressIsInvalid(string address)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(address));

        Assert.Equal("Invalid email format. (Parameter 'address')", exception.Message);
    }

    [Theory(DisplayName = "Should throw ArgumentException when email is empty or whitespace")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Should_ThrowArgumentException_When_EmailIsEmptyOrWhitespace(string address)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(address));
        Assert.Equal("Email cannot be empty. (Parameter 'address')", exception.Message);
    }

    [Fact(DisplayName = "Should throw ArgumentException when email length exceeds 254 characters")]
    public void Should_ThrowArgumentException_When_EmailLengthExceeds254Characters()
    {
        // Arrange
        var address = $"{new string('a', 245)}@example.com";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(address));
        Assert.Equal(
            "Email length exceeds the maximum allowed length of 254 characters. (Parameter 'address')",
            exception.Message
        );
    }
}
