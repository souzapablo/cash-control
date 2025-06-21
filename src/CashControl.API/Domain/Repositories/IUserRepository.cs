using CashControl.API.Domain.Entities;

namespace CashControl.API.Domain.Repositories;

public interface IUserRepository
{
    void Save(User user);
}