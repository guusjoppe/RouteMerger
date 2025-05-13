using RouteMerger.Infrastructure.Enums;

namespace RouteMerger.Infrastructure.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(
        Stream fileStream, 
        string fileName,
        Action<long> onBytesRead,
        FileDirectory fileDirectory = FileDirectory.Uploaded);

    Task<Stream> DownloadFileStreamAsync(
        string fileName,
        FileDirectory fileDirectory = FileDirectory.Uploaded);
    
    void DeleteFile(
        string fileName,
        FileDirectory fileDirectory = FileDirectory.Uploaded);
    
    void DeleteFiles(
        IEnumerable<string> fileNames,
        FileDirectory fileDirectory = FileDirectory.Uploaded);
}