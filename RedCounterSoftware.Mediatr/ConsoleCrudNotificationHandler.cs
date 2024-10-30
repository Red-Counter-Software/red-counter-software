using MediatR;
using RedCounterSoftware.Common;

namespace RedCounterSoftware.Mediatr;

public class ConsoleCrudNotificationHandler<TEntity> : INotificationHandler<CrudNotification<TEntity>>
    where TEntity : class, IIdentifiable
{
    public Task Handle(CrudNotification<TEntity> notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        Console.WriteLine($"Performed CRUD action '{notification.Action}' on entity '{typeof(TEntity).Name}' with Id '{notification.Entity.Id}'");
        return Task.CompletedTask;
    }
}
