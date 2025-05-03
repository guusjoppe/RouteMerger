using DomainRoute = RouteMerger.Domain.Models.Route;
using Route = RouteMerger.Blazor.Models.Route;

namespace RouteMerger.Blazor.Mappers;

public static class RouteMapper
{
    public static DomainRoute ToDomain(this Route route)
    {
        return new DomainRoute
        {
            Name = route.Name,
            Files = route.FileReferences.Select(f => f.ToDomain()).ToList(),
        };
    }
    
    public static Route ToModel(this DomainRoute route)
    {
        return new Route
        {
            Id = route.Id,
            Name = route.Name,
            FileReferences = route.Files.Select(f => f.ToModel()).ToList(),
            LastModifiedAt = route.LastModifiedAt,
        };
    }
}