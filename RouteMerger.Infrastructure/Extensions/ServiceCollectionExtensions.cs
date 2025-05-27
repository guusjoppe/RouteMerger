using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RouteMerger.Infrastructure.Configuration;
using RouteMerger.Infrastructure.Interfaces;
using RouteMerger.Infrastructure.Services;

namespace RouteMerger.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var fileStorageConfiguration = new FileStorageConfiguration();
        configuration.GetSection(FileStorageConfiguration.SectionKey)
            .Bind(fileStorageConfiguration);
        fileStorageConfiguration.Validate();
        services.AddSingleton(fileStorageConfiguration);

        if (fileStorageConfiguration.UseLocalFileStorage)
        {
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
        }
        else
        {
            services.AddScoped<IFileStorageService, AwsS3Service>();
        }
    }
}