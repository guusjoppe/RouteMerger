using Microsoft.Extensions.Logging;
using RouteMerger.Infrastructure.Configuration;
using RouteMerger.Infrastructure.Enums;
using RouteMerger.Infrastructure.Interfaces;

namespace RouteMerger.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly ILogger<IFileStorageService> _logger;
    
    private readonly string _uploadDirectory;
    private readonly string _mergedRoutesDirectory;

    private static readonly byte[] Buffer = new byte[1024 * 10]; // 10 KB buffer

    public LocalFileStorageService(
        ILogger<IFileStorageService> logger,
        FileStorageConfiguration fileStorageConfiguration)
    {
        _logger = logger;
        
        _uploadDirectory = Path.Combine(AppContext.BaseDirectory, fileStorageConfiguration.UploadDirectory);
        Directory.CreateDirectory(_uploadDirectory);
        
        _mergedRoutesDirectory = Path.Combine(AppContext.BaseDirectory, fileStorageConfiguration.MergedRoutesDirectory);
        Directory.CreateDirectory(_mergedRoutesDirectory);
    }

    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        Action<long> onBytesRead,
        FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        var trustedFileName = Guid.NewGuid() + Path.GetExtension(fileName);
        var filePath = GetFilePath(fileDirectory, trustedFileName);

        await using var writeStream = new FileStream(filePath, FileMode.Create);
        int bytesRead;
        while ((bytesRead = await fileStream.ReadAsync(Buffer)) > 0)
        {
            await writeStream.WriteAsync(Buffer.AsMemory(0, bytesRead));
            onBytesRead(bytesRead); // Report bytes read
        }

        _logger.LogInformation("Successfully saved: {FileName} of size {FileSize} kB in {Directory}",
            trustedFileName,
            writeStream.Length / 1024,
            fileDirectory);
        return trustedFileName;
    }
    
    public async Task<Stream> DownloadFileStreamAsync(string fileName,
        FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        var filePath = GetFilePath(fileDirectory, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", fileName);
        }

        _logger.LogInformation("Downloading file: {FileName} from {Directory}",
            fileName,
            fileDirectory);
        return await Task.FromResult(new FileStream(filePath, FileMode.Open, FileAccess.Read));
    }
    
    public void DeleteFile(string fileName,
        FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        var filePath = GetFilePath(fileDirectory, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", fileName);
        }
        
        File.Delete(filePath);
        _logger.LogInformation("Deleted file: {FileName} from {Directory}",
            fileName,
            fileDirectory);
    }
    
    public async Task DeleteFileAsync(string fileName,
        FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        await Task.Run(() => DeleteFile(fileName, fileDirectory));
    }

    public void DeleteFiles(IEnumerable<string> fileNames,
        FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        var fileList = fileNames.ToList();
        
        var tasks = fileList
            .Select(fileName => Task.Run(() => DeleteFile(fileName, fileDirectory)));
        
        Task.WaitAll(tasks.ToArray());
        _logger.LogInformation("Deleted {Count} files from {Directory}",
            fileList.Count,
            fileDirectory);
    }
    
    public async Task DeleteFilesAsync(IEnumerable<string> fileNames,
        FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        await Task.Run(() => DeleteFiles(fileNames, fileDirectory));
    }
    
    private string GetFilePath(FileDirectory fileDirectory, string trustedFileName)
    {
        return fileDirectory switch
        {
            FileDirectory.Uploaded => Path.Combine(_uploadDirectory, trustedFileName),
            FileDirectory.Merged => Path.Combine(_mergedRoutesDirectory, trustedFileName),
            _ => throw new ArgumentOutOfRangeException(nameof(fileDirectory), fileDirectory, null)
        };
    }

}