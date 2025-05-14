namespace RouteMerger.Persistence.Entities;

public class FileReference : BaseEntity
{
    public Guid Id { get; init; }
    public Guid RouteId { get; set; }
    public required string FileName { get; init; }
    public required string UserProvidedName { get; init; }
    public required string RelativePath { get; init; }
    public required string FileExtension { get; init; }
    public bool IsMerged { get; set; }

    public void Update()
    {
        LastUpdatedAt = DateTimeOffset.UtcNow;
    }
}