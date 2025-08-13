using CashControl.App.Abstractions;

namespace CashControl.App.Features.Users;

public class UserException(Error error) : Exception(error.Message)
{
}
