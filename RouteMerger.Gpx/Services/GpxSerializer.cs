using System.Xml;
using System.Xml.Serialization;
using RouteMerger.Gpx.Models;

namespace RouteMerger.Gpx.Services;

public static class GpxSerializer
{
    public static GpxType Deserialize(Stream stream)
    {
        ValidateStream(stream);
        if (stream.Length == 0)
        {
            return new GpxType();
        }

        stream.Seek(0, SeekOrigin.Begin);
        var serializer = new XmlSerializer(typeof(GpxType));
        
        var gpx = (GpxType?)serializer.Deserialize(stream);
        if (gpx == null)
        {
            throw new InvalidOperationException("Failed to deserialize GPX data.");
        }
        
        return gpx;
    }
    
    public static async Task<Stream> SerializeAsync(GpxType gpx)
    {
        var stream = new MemoryStream();
        ArgumentNullException.ThrowIfNull(gpx);

        await using var writer = XmlWriter.Create(
            stream,
            new XmlWriterSettings
            {
                Async = true,
                Indent = true,
                OmitXmlDeclaration = true,
            });
        
        var serializer = new XmlSerializer(typeof(GpxType));
        serializer.Serialize(writer, gpx);
        await writer.FlushAsync();
        
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    private static void ValidateStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanSeek)
        {
            throw new ArgumentException("Stream must support seeking.", nameof(stream));
        }
    }
}