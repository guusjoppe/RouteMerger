using System.ComponentModel.DataAnnotations;

namespace RouteMerger.Blazor.Models;

public class Route
{
    public Guid Id { get; init; }
    
    [Required(ErrorMessage = "Route name is required")]
    [StringLength(100, ErrorMessage = "Route name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "At least one file is required")]
    public List<FileReference> FileReferences { get; set; } = [];
    
    public DateTimeOffset LastModifiedAt { get; init; }
}