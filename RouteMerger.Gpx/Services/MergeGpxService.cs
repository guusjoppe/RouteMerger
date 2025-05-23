using RouteMerger.Gpx.Factory;
using RouteMerger.Gpx.Models;

namespace RouteMerger.Gpx.Services;

public static class MergeGpxService
{
    public static async Task<Stream> MergeGpxStreamsAsync(Stream[] fileStreams, string routeName)
    {
        var mergedGpx = MergeGpx(fileStreams, routeName);

        return await GpxSerializer.SerializeAsync(mergedGpx);
    }
    
    public static GpxType MergeGpx(Stream[] gpxStreams, string routeName)
    {
        var gpxs = gpxStreams.Select(GpxSerializer.Deserialize).ToArray();
        
        return MergeGpx(gpxs, routeName);
    }
    
    public static GpxType MergeGpx(GpxType[] gpxs, string routeName)
    {
        var tracks = new List<TrkType>();
        
        foreach (var gpx in gpxs)
        {
            tracks.AddRange(gpx.trk);
        }
        
        return GpxFactory.Create(routeName, tracks.ToArray());
    }
}