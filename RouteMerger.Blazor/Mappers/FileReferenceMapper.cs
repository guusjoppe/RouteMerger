using RouteMerger.Blazor.Models;
using DomainFileReference = RouteMerger.Domain.Models.FileReference;

namespace RouteMerger.Blazor.Mappers;

public static class FileReferenceMapper
{
    public static DomainFileReference ToDomain(this FileReference file)
    {
        return new DomainFileReference
        {
            Id = file.Id,
            FileName = file.FileName,
            UserProvidedName = file.UserProvidedName,
            FileExtension = file.FileExtension,
        };
    }
    
    public static FileReference ToModel(this DomainFileReference file)
    {
        return new FileReference
        {
            Id = file.Id,
            FileName = file.FileName,
            UserProvidedName = file.UserProvidedName,
            FileExtension = file.FileExtension,
            LastModifiedAt = file.LastModifiedAt,
        };
    }
}