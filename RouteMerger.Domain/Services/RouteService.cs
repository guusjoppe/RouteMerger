using System.Xml;
using System.Xml.Serialization;
using RouteMerger.Domain.Interfaces;
using RouteMerger.Domain.Models;
using RouteMerger.Domain.Utilities.XmlSchemas.Schemas.Models;
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
        var serializer = new XmlSerializer(typeof(gpxType));
        var route = await routeRepository.GetAsync(id);
        var fileStreams = await fileReferenceService.GetFileStreamsAsync(route.Files.Select(f => f.Id));

        var trks = new List<trkType>();
        foreach (var fileStream in fileStreams)
        {
            var gpx = (gpxType?)serializer.Deserialize(fileStream);
            if (gpx == null)
            {
                throw new InvalidOperationException("Failed to deserialize GPX file.");
            }

            var trk = gpx.trk;
            if (trk == null || trk.Length == 0)
            {
                throw new InvalidOperationException("No track found in GPX file.");
            }
            
            trks.AddRange(trk);
        }

        var mergedGpx = new gpxType
        {
            metadata = new metadataType
            {
                name = route.Name,
                time = DateTime.UtcNow,
            },
            trk = trks.ToArray(),
            creator = "RouteMerger",
        };
        
        var mergedFileName = $"{route.Name}.gpx";
        var mergedFileStream = new MemoryStream();
        await using var writer = XmlWriter.Create(
            mergedFileStream,
            new XmlWriterSettings
            {
                Async = true,
            });
        serializer.Serialize(writer, mergedGpx);
        await writer.FlushAsync();

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