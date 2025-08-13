using AiTesting.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence;

public class AiTestingContext(DbContextOptions<AiTestingContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AiTestingContext).Assembly);
    }
}
