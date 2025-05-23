using Microsoft.AspNetCore.Components.Forms;
using RouteMerger.Domain.Interfaces;

namespace RouteMerger.Blazor.Adapters;

public class BrowserFileAdapter : IFileUpload
{
    private readonly IBrowserFile _browserFile;

    public BrowserFileAdapter(IBrowserFile browserFile)
    {
        _browserFile = browserFile;
    }

    public string Name => _browserFile.Name;
    public long Size => _browserFile.Size;
    public Stream OpenReadStream(long maxAllowedSize) => _browserFile.OpenReadStream(maxAllowedSize);
}