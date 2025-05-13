using RouteMerger.Domain.Exceptions;
using RouteMerger.Domain.Interfaces;
using RouteMerger.Domain.Models;
using RouteMerger.Persistence.Data;
using RouteMerger.Persistence.Mappers;

namespace RouteMerger.Persistence.Repositories;

public class RouteRepository(RouteMergerDbContext context) : IRouteRepository
{
    public async Task<Route> GetAsync(Guid id)
    {
        var route = await FindRouteAsync(id);
        if (route == null)
        {
            throw new EntityNotFoundException(id, nameof(route));
        }

        return route.ToDomain();
    }

    public async Task<IEnumerable<Route>> GetAllAsync()
    {
        var routes = await context.Routes
            .Where(r => r.DeletedAt == null)
            .Include(r => r.FileReferences
                .Where(fr => fr.DeletedAt == null))
            .Include(r => r.MergedFileReference)
            .OrderByDescending(r => r.LastUpdatedAt)
            .ToListAsync();

        return routes.Select(r => r.ToDomain());
    }

    public async Task<Route> AddAsync(Route route)
    {
        var entity = route.ToEntity();
        
        entity.UpdateFileReferences(route.Files.Select(f => f.ToEntity()).ToList());
        entity.Create();
        
        context.Routes.Add(entity);
        await context.SaveChangesAsync();

        return entity.ToDomain();
    }

    public async Task<Route> UpdateAsync(Guid id, Route route)
    {
        var entity = await FindRouteAsync(id);
        if (entity == null)
        {
            throw new EntityNotFoundException(id, nameof(route));
        }
        
        entity.UpdateFileReferences(route.Files.Select(f => f.ToEntity()).ToList());
        entity.Update(route);
        
        context.Routes.Update(entity);
        await context.SaveChangesAsync();
        
        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id)
    {
        await context.Routes
            .Where(r => r.Id == id && r.DeletedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(
                r => r.DeletedAt,
                DateTimeOffset.UtcNow));
    }

    public async Task<FileReference> UpdateMergedRouteReference(
        Guid id,
        FileReference mergedFileReference)
    {
        var route = await FindRouteAsync(id);
        if (route == null)
        {
            throw new EntityNotFoundException(id, nameof(Route));
        }
        
        route.UpdateMergedFileReference(mergedFileReference.ToEntity());
        await context.SaveChangesAsync();

        return route.MergedFileReference!.ToDomain();
    }

    private async Task<Entities.Route?> FindRouteAsync(Guid id)
    {
        return await context.Routes
            .Include(r => r.FileReferences
                .Where(fr => fr.DeletedAt == null))
            .Include(r => r.MergedFileReference)
            .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);
    }
}