using RouteMerger.Persistence.Entities;
using DomainRoute = RouteMerger.Domain.Models.Route;

namespace RouteMerger.Persistence.Mappers;

public static class RouteMapper
{
    public static DomainRoute ToDomain(this Route entity)
    {
        return new DomainRoute
        {
            Id = entity.Id,
            Name = entity.Name,
            Files = entity.FileReferences.Select(f => f.ToDomain()).ToList(),
            MergedFileReference = entity.MergedFileReference?.ToDomain(),
            LastModifiedAt = entity.LastUpdatedAt,
        };
    }
    
    public static Route ToEntity(this DomainRoute domain)
    {
        return new Route
        {
            Id = domain.Id,
            Name = domain.Name,
        };
    }
}