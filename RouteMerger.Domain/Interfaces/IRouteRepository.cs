using RouteMerger.Domain.Models;

namespace RouteMerger.Domain.Interfaces;

public interface IRouteRepository
{
    Task<Route> GetAsync(Guid id);
    Task<IEnumerable<Route>> GetAllAsync();
    Task<Route> AddAsync(Route route);
    Task<Route> UpdateAsync(Guid id, Route route);
    Task DeleteAsync(Guid id);
    Task<FileReference> UpdateMergedRouteReference(Guid id, FileReference mergedFileReference);
}