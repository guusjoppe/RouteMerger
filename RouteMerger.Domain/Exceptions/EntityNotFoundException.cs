namespace RouteMerger.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    private const string DefaultMessage = "Entity not found.";
    private Guid Id { get; }
    private string EntityName { get; }
    
    public override string Message => $"{DefaultMessage} Entity: {EntityName}, Id: {Id}";
    
    public EntityNotFoundException(
        Guid id,
        string entityName)
    {
        Id = id;
        EntityName = entityName;
    }
}