using CashControl.API.Abstractions;
using CashControl.API.Domain.Entities;
using CashControl.API.Domain.Repositories;

namespace CashControl.API.Data.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public void Save(User user) => context.Users.Add(user);
}