namespace CashControl.App.Features.Users;

public interface IUserRepository
{
    Task<bool> ExistWithEmailAsync(string email, CancellationToken cancellationToken = default);
    void Register(User user);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}