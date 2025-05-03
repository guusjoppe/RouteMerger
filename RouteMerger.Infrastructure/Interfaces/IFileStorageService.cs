namespace RouteMerger.Infrastructure.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(
        Stream fileStream, 
        string fileName,
        Action<long> onBytesRead);

    Task<Stream> DownloadFileStreamAsync(string fileName);
    
    void DeleteFile(string fileName);
    
    void DeleteFiles(IEnumerable<string> fileNames);
}