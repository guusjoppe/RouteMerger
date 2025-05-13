using System.Xml;
using System.Xml.Schema;
using Microsoft.AspNetCore.Components.Forms;
using RouteMerger.Domain.Interfaces;
using RouteMerger.Domain.Models;
using RouteMerger.Domain.Utilities;
using RouteMerger.Domain.Utilities.XmlSchemas;
using RouteMerger.Infrastructure.Enums;
using RouteMerger.Infrastructure.Interfaces;

namespace RouteMerger.Domain.Services;

public class FileReferenceService(
    IFileStorageService fileStorageService,
    IFileReferenceRepository fileReferenceRepository)
{
    private const long MaxFileSize = 1024 * 1024 * 10; // 10 MB
    private static readonly string[] AllowedExtensions = [".gpx"];
    
    public async Task<ICollection<FileReference>> ProcessFilesAsync(
        IBrowserFile[] files,
        Action<decimal> onProgress)
    {
        var fileReferences = new List<FileReference>();
        var totalFileSize = files.Sum(file => file.Size);
        var totalRead = 0L;

        foreach (var file in files)
        {
            var fileExtension = Path.GetExtension(file.Name).ToLowerInvariant();
            await ValidateFileAsync(fileExtension, file);

            await using var readStream = file.OpenReadStream(MaxFileSize);
            var storedFileName = await fileStorageService.SaveFileAsync(
                readStream,
                file.Name,
                bytesRead =>
                {
                    totalRead += bytesRead;
                    var progress = ProgressCalculator.CalculateProgress(totalRead, totalFileSize);
                    onProgress(progress);
                });

            fileReferences.Add(new FileReference
            {
                FileName = storedFileName,
                UserProvidedName = file.Name,
                FileExtension = fileExtension,
            });
        }

        return fileReferences;
    }

    public async Task<FileReference> ProcessFileStreamAsync(
        Stream fileStream, 
        string fileName,
        Action<decimal> onProgress,
        FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        var totalFileSize = fileStream.Length;
        var totalRead = 0L;
        
        var storedFileName = await fileStorageService.SaveFileAsync(
            fileStream,
            fileName,
            bytesRead =>
            {
                totalRead += bytesRead;
                var progress = ProgressCalculator.CalculateProgress(totalRead, totalFileSize);
                onProgress(progress);
            },
            fileDirectory);

        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        var fileReference = new FileReference
        {
            FileName = storedFileName,
            UserProvidedName = fileName,
            FileExtension = fileExtension
        };

        return fileReference;
    }
    
    public async Task<Stream[]> GetFileStreamsAsync(IEnumerable<Guid> fileReferenceIds)
    {
        var fileReferences = await fileReferenceRepository.GetAsync(fileReferenceIds);
        
        var streams = fileReferences.Select(fr => 
            fileStorageService.DownloadFileStreamAsync(fr.FileName)).ToArray();
        
        return await Task.WhenAll(streams);
    }
    
    public async Task DeleteFileAsync(Guid fileReferenceId)
    {
        var fileReference = await fileReferenceRepository.GetAsync(fileReferenceId);
        
        fileStorageService.DeleteFile(fileReference.FileName);
        
        await fileReferenceRepository.DeleteAsync(fileReferenceId);
    }
    
    public async Task DeleteFilesFromRouteAsync(Guid routeId)
    {
        var fileReferences = await fileReferenceRepository.GetByRouteIdAsync(routeId);
        
        fileStorageService.DeleteFiles(fileReferences.Select(fr => fr.FileName));
        
        await fileReferenceRepository.DeleteAsync(fileReferences.Select(fr => fr.Id));
    }

    private static async Task ValidateFileAsync(string fileExtension, IBrowserFile file)
    {
        if (!AllowedExtensions.Contains(fileExtension))
        {
            throw new InvalidOperationException($"File extension '{fileExtension}' is not allowed.");
        }

        if (file.Size > MaxFileSize)
        {
            throw new InvalidOperationException("File size exceeds the maximum allowed limit.");
        }
        
        var validationErrors = await ValidateGpxContentAsync(file);
        if (validationErrors != null)
        {
            throw new InvalidOperationException($"GPX file validation failed: {validationErrors}");
        }
    }
    
    private static async Task<string?> ValidateGpxContentAsync(IBrowserFile file)
    {
        var schemaSet = XmlSchemaSetFactory.CreateGpxXmlSchemaSet();
        var validationErrors = new List<string>();
        
        try
        {
            await using var fileStream = file.OpenReadStream(MaxFileSize);
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset the stream position for reading
            var settings = new XmlReaderSettings
            {
                Async = true,
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings,
            };

            settings.ValidationEventHandler += (_, args) =>
            {
                validationErrors.Add($"Line {args.Exception.LineNumber}, Position {args.Exception.LinePosition}: {args.Message}");
            };
            
            using var reader = XmlReader.Create(memoryStream, settings);
            while (await reader.ReadAsync()) { } // Parse the entire file to validate
            
            return validationErrors.Count != 0
                ? string.Join(Environment.NewLine, validationErrors)
                : null;
        }
        catch (XmlException ex)
        {
            throw new InvalidOperationException($"Invalid XML format: {ex.Message}");
        }
        catch (XmlSchemaValidationException ex)
        {
            throw new InvalidOperationException($"Invalid GPX schema: {ex.Message}");
        }
    }
}