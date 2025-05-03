using RouteMerger.Domain.Exceptions;
using RouteMerger.Domain.Interfaces;
using RouteMerger.Domain.Models;
using RouteMerger.Persistence.Data;
using RouteMerger.Persistence.Mappers;

namespace RouteMerger.Persistence.Repositories;

public class FileReferenceRepository(
    RouteMergerDbContext context) : IFileReferenceRepository
{
    public async Task<ICollection<FileReference>> GetAsync(IEnumerable<Guid> ids)
    {
        var fileReferences = await context.FileReferences
            .Where(fr => ids.Contains(fr.Id) && fr.DeletedAt == null)
            .ToListAsync();

        return fileReferences
            .Select(fr => fr.ToDomain())
            .ToList();
    }

    public async Task<FileReference> GetAsync(Guid id)
    {
        var fileReference = await context.FileReferences
            .Where(fr => fr.Id == id && fr.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (fileReference == null)
        {
            throw new EntityNotFoundException(id, nameof(FileReference));
        }
        
        return fileReference.ToDomain();
    }

    public async Task DeleteAsync(Guid id)
    {
        await context.FileReferences
            .Where(fr => fr.Id == id && fr.DeletedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(
                fr => fr.DeletedAt,
                DateTimeOffset.UtcNow));
    }
    
    public async Task DeleteAsync(IEnumerable<Guid> ids)
    {
        await context.FileReferences
            .Where(fr => ids.Contains(fr.Id) && fr.DeletedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(
                fr => fr.DeletedAt,
                DateTimeOffset.UtcNow));
    }

    public async Task<ICollection<FileReference>> GetByRouteIdAsync(Guid routeId)
    {
        var fileReferences = await context.FileReferences
            .Where(fr => fr.RouteId == routeId && fr.DeletedAt == null)
            .ToListAsync();

        return fileReferences
            .Select(fr => fr.ToDomain())
            .ToList();
    }
}