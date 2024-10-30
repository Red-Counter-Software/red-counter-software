using System.Collections.ObjectModel;
using MediatR;
using RedCounterSoftware.Common;
using RedCounterSoftware.Common.Validation;

namespace RedCounterSoftware.Mediatr;

/// <summary>
/// Handles the update of an entity.
/// </summary>
/// <typeparam name="TDto">The type of the DTO.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
/// <param name="dataContext">The data context.</param>
public class UpdateEntityHandler<TDto, TEntity, TId>(IMediator mediator, IDataContext<TEntity> dataContext)
    : IRequestHandler<UpdateEntityRequest<TDto, TEntity, TId>, Result<TEntity>>
    where TDto : class
    where TEntity : class, IIdentifiable<TId>, new()
{
    private readonly IMediator mediator = mediator ?? throw new ArgumentNullException(nameof(mediator), "mediator is required");
    private readonly IDataContext<TEntity> dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext), "dataContext is required");

    /// <summary>
    /// Handles the update entity request.
    /// </summary>
    /// <param name="request">The update entity request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Result<TEntity>> Handle(UpdateEntityRequest<TDto, TEntity, TId> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check if the entity exists
        var entityExists = await this.dataContext.ExistsBy(e => e.Id, request.Id, cancellationToken).ConfigureAwait(false);
        if (!entityExists)
        {
            var failures = new Collection<Failure>
            {
                new(nameof(request.Id), $"Entity with ID {request.Id} was not found.")
            };
            return new Result<TEntity>(null, failures);
        }

        // Update the entity
        var updatedEntity = await this.dataContext.Update(request.Entity, request.Id, cancellationToken).ConfigureAwait(false);
        await this.mediator.Publish(new CrudNotification<TEntity>(CrudNotification<TEntity>.CrudAction.Update, updatedEntity), cancellationToken).ConfigureAwait(false);
        return new Result<TEntity>(updatedEntity, []);
    }
}
