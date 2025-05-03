using Microsoft.Extensions.DependencyInjection;
using RouteMerger.Infrastructure.Interfaces;
using RouteMerger.Infrastructure.Services;

namespace RouteMerger.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
    }
}