using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RouteMerger.Domain.Interfaces;
using RouteMerger.Persistence.Data;
using RouteMerger.Persistence.Repositories;

namespace RouteMerger.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);

        AddRepositories(services);
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<IFileReferenceRepository, FileReferenceRepository>();
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RouteMergerDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("RouteMergerDbContext")));
        
        // run missing migrations on the database if it is in development mode
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RouteMergerDbContext>();
            dbContext.Database.Migrate();
        }
    }
}