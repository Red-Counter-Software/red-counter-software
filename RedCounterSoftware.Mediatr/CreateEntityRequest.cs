using MediatR;
using RedCounterSoftware.Common;
using RedCounterSoftware.Common.Validation;

namespace RedCounterSoftware.Mediatr
{
    public class CreateEntityRequest<TDto, TEntity, TId>(TDto dto, TId id, Func<TEntity, Task<Failure[]>>? additionalValidation = null)
        : CrudRequestBase<TDto, TEntity>(dto, additionalValidation), IRequest<Result<TEntity>>
        where TDto : class
        where TEntity : class, IIdentifiable<TId>, new()
    {
        public TId Id { get; } = id ?? throw new ArgumentNullException(nameof(id), "id is required");
    }
}
