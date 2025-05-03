using Microsoft.Extensions.DependencyInjection;
using RouteMerger.Domain.Services;

namespace RouteMerger.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<RouteService>();
        services.AddScoped<FileReferenceService>();
    }
}