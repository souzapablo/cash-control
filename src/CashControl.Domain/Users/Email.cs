using System.Text.RegularExpressions;

namespace CashControl.Domain.Users;

public partial record Email
{
    private Email(string address)
    {
        Address = address;
    }

    public string Address { get; init; }

    public static Email Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Email cannot be empty.", nameof(address));

        address = address.Trim().ToLowerInvariant();

        if (address.Length > 254)
            throw new ArgumentException(
                "Email length exceeds the maximum allowed length of 254 characters.",
                nameof(address)
            );

        if (!EmailValidationRegex().IsMatch(address))
            throw new ArgumentException("Invalid email format.", nameof(address));

        return new Email(address);
    }

    public override string ToString() => Address;

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailValidationRegex();
}
