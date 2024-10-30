using MediatR;
using RedCounterSoftware.Common.Validation;

namespace RedCounterSoftware.Mediatr;

public abstract class CrudRequestBase<TDto, TEntity>(TDto dto, Func<TEntity, Task<Failure[]>>? additionalValidation = null)
    : IRequest<Result<TEntity>>
    where TDto : class
    where TEntity : class, new()
{
    public TDto Dto { get; } = dto ?? throw new ArgumentNullException(nameof(dto), "dto is required");

    public Func<TEntity, Task<Failure[]>>? AdditionalValidation { get; } = additionalValidation;

    public TEntity Entity { get; set; } = new();

    public Result<TEntity> Result { get; set; } = new Result<TEntity>(null, []);
}
