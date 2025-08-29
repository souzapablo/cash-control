using CashControl.App.Features.Users;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CashControl.App.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> context)
    : DbContext(context)
{
    public DbSet<User> Users { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}