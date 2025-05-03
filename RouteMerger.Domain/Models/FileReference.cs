namespace RouteMerger.Domain.Models;

public class FileReference
{
    public Guid Id { get; init; }
    public required string FileName { get; init; }
    public required string UserProvidedName { get; init; }
    public required string FileExtension { get; init; }
    public string RelativePath { get; set; } = string.Empty;
    public DateTimeOffset LastModifiedAt { get; init; }
}