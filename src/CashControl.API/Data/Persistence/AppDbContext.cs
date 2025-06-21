using System.Reflection;
using CashControl.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashControl.API.Data.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> context) : DbContext(context)
{
    public DbSet<User> Users { get; private set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}