using Microsoft.Extensions.Logging;
using RouteMerger.Infrastructure.Interfaces;

namespace RouteMerger.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly ILogger<IFileStorageService> _logger;
    
    private readonly string _uploadDirectory;

    private static readonly byte[] Buffer = new byte[1024 * 10]; // 10 KB buffer

    public LocalFileStorageService(ILogger<IFileStorageService> logger)
    {
        _logger = logger;
        
        _uploadDirectory = Path.Combine(AppContext.BaseDirectory, "uploads");
        Directory.CreateDirectory(_uploadDirectory); // Ensure the directory exists
    }

    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        Action<long> onBytesRead)
    {
        var trustedFileName = Guid.NewGuid() + Path.GetExtension(fileName);
        var filePath = Path.Combine(_uploadDirectory, trustedFileName);

        await using var writeStream = new FileStream(filePath, FileMode.Create);
        int bytesRead;
        while ((bytesRead = await fileStream.ReadAsync(Buffer)) > 0)
        {
            await writeStream.WriteAsync(Buffer.AsMemory(0, bytesRead));
            onBytesRead(bytesRead); // Report bytes read
        }

        _logger.LogInformation("Successfully saved: {FileName} of size {FileSize} kB",
            trustedFileName,
            writeStream.Length / 1024);
        return trustedFileName;
    }

    public async Task<Stream> DownloadFileStreamAsync(string fileName)
    {
        var filePath = Path.Combine(_uploadDirectory, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", fileName);
        }

        _logger.LogInformation("Downloading file: {FileName}", fileName);
        return await Task.FromResult(new FileStream(filePath, FileMode.Open, FileAccess.Read));
    }
    
    public void DeleteFile(string fileName)
    {
        var filePath = Path.Combine(_uploadDirectory, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", fileName);
        }
        
        File.Delete(filePath);
        _logger.LogInformation("Deleted file: {FileName}", fileName);
    }

    public void DeleteFiles(IEnumerable<string> fileNames)
    {
        var fileList = fileNames.ToList();
        
        var tasks = fileList
            .Select(fileName => Task.Run(() => DeleteFile(fileName)));
        
        Task.WaitAll(tasks.ToArray());
        _logger.LogInformation("Deleted {Count} files", fileList.Count);
    }
}