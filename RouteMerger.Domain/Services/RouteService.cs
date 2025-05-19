using System.Xml;
using System.Xml.Serialization;
using RouteMerger.Domain.Interfaces;
using RouteMerger.Domain.Models;
using RouteMerger.Gpx.Services;
using RouteMerger.Infrastructure.Enums;

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

    public async Task<FileReference> MergeRoutesAsync(
        Guid id,
        Action<decimal> onUploadProgress)
    {
        var route = await routeRepository.GetAsync(id);
        var fileStreams = await fileReferenceService.GetFileStreamsAsync(route.Files.Select(f => f.Id));

        var routeName = route.Name;
        var mergedFileStream = await MergeGpxService.MergeGpxStreamsAsync(fileStreams, routeName);

        var mergedFileName = $"{routeName}.gpx";
        var mergedFileReference = await fileReferenceService.ProcessFileStreamAsync(
            mergedFileStream,
            mergedFileName,
            onUploadProgress,
            FileDirectory.Merged);
        
        return await routeRepository.UpdateMergedRouteReference(
            id,
            mergedFileReference);
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