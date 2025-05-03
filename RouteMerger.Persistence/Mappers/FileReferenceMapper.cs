using RouteMerger.Persistence.Entities;
using DomainFileReference = RouteMerger.Domain.Models.FileReference;

namespace RouteMerger.Persistence.Mappers;

public static class FileReferenceMapper
{
    public static DomainFileReference ToDomain(this FileReference fileReference)
    {
        return new DomainFileReference
        {
            Id = fileReference.Id,
            FileName = fileReference.FileName,
            UserProvidedName = fileReference.UserProvidedName,
            RelativePath = fileReference.RelativePath,
            FileExtension = fileReference.FileExtension,
            LastModifiedAt = fileReference.LastUpdatedAt,
        };
    }
    
    public static FileReference ToEntity(this DomainFileReference fileReference)
    {
        return new FileReference
        {
            Id = fileReference.Id,
            FileName = fileReference.FileName,
            UserProvidedName = fileReference.UserProvidedName,
            RelativePath = fileReference.RelativePath,
            FileExtension = fileReference.FileExtension,
        };
    }
}