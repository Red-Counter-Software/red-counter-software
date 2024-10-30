using MediatR;

namespace RedCounterSoftware.Mediatr;

public class CrudNotification<TEntity>(CrudNotification<TEntity>.CrudAction action, TEntity entity)
    : INotification
    where TEntity : class
{
    public enum CrudAction
    {
        Create,
        Update,
        Delete
    }

    public CrudAction Action { get; } = action;

    public TEntity Entity { get; } = entity ?? throw new ArgumentNullException(nameof(entity), "entity is required");
}
