using MediatR;
using RedCounterSoftware.Common;
using RedCounterSoftware.Common.Validation;

namespace RedCounterSoftware.Mediatr;

public class CreateEntityHandler<TDto, TEntity, TId>(IMediator mediator, IDataContext<TEntity> dataContext)
    : IRequestHandler<CreateEntityRequest<TDto, TEntity, TId>, Result<TEntity>>
    where TDto : class
    where TEntity : class, IIdentifiable<TId>, new()
{
    private readonly IMediator mediator = mediator ?? throw new ArgumentNullException(nameof(mediator), "mediator is required");
    private readonly IDataContext<TEntity> dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext), "dataContext is required");

    public async Task<Result<TEntity>> Handle(CreateEntityRequest<TDto, TEntity, TId> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var output = await this.dataContext.Add(c => c.Id, request.Id, request.Entity, cancellationToken).ConfigureAwait(false);
        await this.mediator.Publish(new CrudNotification<TEntity>(CrudNotification<TEntity>.CrudAction.Create, output), cancellationToken).ConfigureAwait(false);
        return new Result<TEntity>(output, []);
    }
}
