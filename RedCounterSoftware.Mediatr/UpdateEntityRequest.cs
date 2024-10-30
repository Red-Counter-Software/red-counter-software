using MediatR;
using RedCounterSoftware.Common;
using RedCounterSoftware.Common.Validation;

namespace RedCounterSoftware.Mediatr;

/// <summary>
/// Request to update an entity.
/// </summary>
/// <typeparam name="TDto">The type of the DTO.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
/// <param name="dto">The DTO.</param>
/// <param name="id">The entity's identifier.</param>
/// <param name="additionalValidation">Additional validation to perform.</param>
public class UpdateEntityRequest<TDto, TEntity, TId>(TDto dto, TId id, Func<TEntity, Task<Failure[]>>? additionalValidation = null)
    : CrudRequestBase<TDto, TEntity>(dto, additionalValidation), IRequest<Result<TEntity>>
    where TDto : class
    where TEntity : class, IIdentifiable<TId>, new()
{
    public TId Id { get; } = id ?? throw new ArgumentNullException(nameof(id), "id is required");
}
