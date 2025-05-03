using RouteMerger.Domain.Models;

namespace RouteMerger.Domain.Interfaces;

public interface IFileReferenceRepository
{
    Task<ICollection<FileReference>> GetAsync(IEnumerable<Guid> id);
    Task<FileReference> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task DeleteAsync(IEnumerable<Guid> id);
    Task<ICollection<FileReference>> GetByRouteIdAsync(Guid routeId);
}