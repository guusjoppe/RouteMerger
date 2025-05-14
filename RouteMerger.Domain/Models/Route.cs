namespace RouteMerger.Domain.Models;

public class Route
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public DateTimeOffset LastModifiedAt { get; init; }
    public ICollection<FileReference> Files { get; init; } = [];
    public FileReference? MergedFileReference { get; set; }
}