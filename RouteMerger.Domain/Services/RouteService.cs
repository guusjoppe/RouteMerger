using RouteMerger.Domain.Interfaces;
using RouteMerger.Domain.Models;

namespace RouteMerger.Domain.Services;

public class RouteService(
    IRouteRepository routeRepository,
    FileReferenceService fileReferenceService)
{
    public async Task<Route> GetAsync(Guid id)
    {
        return await routeRepository.GetAsync(id);
    }
    
    public async Task<IEnumerable<Route>> GetAllAsync()
    {
        return await routeRepository.GetAllAsync();
    }
    
    public async Task<Route> AddAsync(Route route)
    {
        return await routeRepository.AddAsync(route);
    }
    
    public async Task<Route> UpdateAsync(Guid id, Route route)
    {
        return await routeRepository.UpdateAsync(id, route);
    }
    
    public async Task DeleteAsync(Guid id)
    {
        await fileReferenceService.DeleteFilesFromRouteAsync(id);
        await routeRepository.DeleteAsync(id);
    }
}