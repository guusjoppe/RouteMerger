using RouteMerger.Gpx.Models;

namespace RouteMerger.Gpx.Factory;

public static class GpxFactory
{
    private const string GpxCreator = "RouteMerger";
    
    public static GpxType Create(
        string name,
        TrkType[] tracks)
    {
        return new GpxType
        {
            metadata = new MetadataType
            {
                name = name,
                time = DateTime.UtcNow,
            },
            trk = tracks,
            creator = GpxCreator,
        };
    }
}