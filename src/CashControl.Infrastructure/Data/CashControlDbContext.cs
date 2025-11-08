using System.Reflection;
using CashControl.Domain.Accounts;
using CashControl.Domain.Categories;
using CashControl.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace CashControl.Infrastructure.Data;

public class CashControlDbContext : DbContext
{
    public CashControlDbContext(DbContextOptions<CashControlDbContext> options)
        : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
