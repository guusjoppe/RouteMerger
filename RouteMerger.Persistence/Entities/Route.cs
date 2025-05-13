using DomainRoute = RouteMerger.Domain.Models.Route;

namespace RouteMerger.Persistence.Entities;

public class Route : BaseEntity
{
    public Guid Id { get; init; }
    public required string Name { get; set; }
    public ICollection<FileReference> FileReferences { get; init; } = [];
    
    public Guid? MergedFileReferenceId { get; set; }
    public FileReference? MergedFileReference { get; set; }
    
    public void Update(DomainRoute route)
    {
        Name = route.Name;
        LastUpdatedAt = DateTimeOffset.UtcNow;
    }

    public override void Delete()
    {
        DeleteFileReferences();

        DeletedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateFileReferences(ICollection<FileReference> fileReferences)
    {
        var existingFileReferences = FileReferences.ToList();
        foreach (var fileReference in fileReferences)
        {
            if (existingFileReferences.Select(fr => fr.Id).Contains(fileReference.Id))
            {
                continue;
            }
            
            fileReference.Update();
            FileReferences.Add(fileReference);
        }
        
        LastUpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void UpdateMergedFileReference(FileReference mergedFileReference)
    {
        MergedFileReference?.Delete();

        MergedFileReference = mergedFileReference;
        mergedFileReference.RouteId = Id;
        
        LastUpdatedAt = DateTimeOffset.UtcNow;
    }
    
    private void DeleteFileReferences()
    {
        foreach (var fileReference in FileReferences)
        {
            fileReference.Delete();
        }
    }
}