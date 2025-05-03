using System.Xml;
using System.Xml.Schema;

namespace RouteMerger.Domain.Utilities.XmlSchemas;

public static class XmlSchemaSetFactory
{
    private static readonly string SchemaPath = Path.Combine(
        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty,
        "Utilities",
        "XmlSchemas",
        "Schemas");
    
    private static readonly string GpxSchemaPath = Path.Combine(
        SchemaPath,
        "gpx.xsd");
    
    private static readonly string GarminGpxExtensionV3Path = Path.Combine(
        SchemaPath,
        "GarminGpxExtensionsv3.xsd");
    
    private static readonly string GarminTrackPointExtensionV1Path = Path.Combine(
        SchemaPath,
        "GarminTrackPointExtensionv1.xsd");

    
    public static XmlSchemaSet CreateGpxXmlSchemaSet()
    {
        var schemaSet = new XmlSchemaSet();
        
        // dynamically load the GPX schemas
        var gpxSchemaDocument = XmlReader.Create(File.OpenRead(GpxSchemaPath));
        var garminGpxExtensionDocument = XmlReader.Create(File.OpenRead(GarminGpxExtensionV3Path));
        var garminTrackPointExtensionDocument = XmlReader.Create(File.OpenRead(GarminTrackPointExtensionV1Path));
        
        schemaSet.Add(null, gpxSchemaDocument);
        schemaSet.Add(null, garminGpxExtensionDocument);
        schemaSet.Add(null, garminTrackPointExtensionDocument);
        
        return schemaSet;
    }
}