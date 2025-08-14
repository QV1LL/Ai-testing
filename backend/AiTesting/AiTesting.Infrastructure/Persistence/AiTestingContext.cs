using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence;

public class AiTestingContext(DbContextOptions<AiTestingContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AiTestingContext).Assembly);
    }
}
