using MediatR;
using RedCounterSoftware.Common;

namespace RedCounterSoftware.Mediatr;

public class DeleteEntityHandler<TEntity, TId>(IMediator mediator, IDataContext<TEntity> dataContext)
    : IRequestHandler<DeleteEntityRequest<TEntity, TId>>
    where TEntity : class, IIdentifiable<TId>
{
    private readonly IMediator mediator = mediator ?? throw new ArgumentNullException(nameof(mediator), "mediator is required");
    private readonly IDataContext<TEntity> dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext), "dataContext is required");

    public async Task Handle(DeleteEntityRequest<TEntity, TId> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = await this.dataContext.GetBy(c => c.Id, request.Id, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            return;
        }

        await this.dataContext.Delete(c => c.Id, request.Id, request.HardDelete, cancellationToken).ConfigureAwait(false);
        await this.mediator.Publish(new CrudNotification<TEntity>(CrudNotification<TEntity>.CrudAction.Delete, entity), cancellationToken).ConfigureAwait(false);
    }
}
