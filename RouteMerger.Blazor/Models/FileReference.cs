namespace RouteMerger.Blazor.Models;

public class FileReference
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public required string UserProvidedName { get; init; }
    public required string FileExtension { get; init; }
    public DateTimeOffset LastModifiedAt { get; init; }
}