namespace RouteMerger.Domain.Interfaces;

public interface IFileUpload
{
    string Name { get; }
    long Size { get; }
    Stream OpenReadStream(long maxAllowedSize);
}