using Microsoft.EntityFrameworkCore.Design;

namespace RouteMerger.Persistence.Data;

public class RouteMergerDbContextFactory : IDesignTimeDbContextFactory<RouteMergerDbContext>
{
    public RouteMergerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RouteMergerDbContext>();
        optionsBuilder.UseNpgsql("RouteMergerDbContext");

        return new RouteMergerDbContext(optionsBuilder.Options);
    }
}