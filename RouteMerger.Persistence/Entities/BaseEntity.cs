namespace RouteMerger.Persistence.Entities;

public abstract class BaseEntity
{
    public DateTimeOffset? DeletedAt { get; protected set; }

    public DateTimeOffset LastUpdatedAt { get; protected set; }

    public virtual void Create()
    {
        LastUpdatedAt = DateTimeOffset.UtcNow;
    }

    public virtual void Delete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
    }
}