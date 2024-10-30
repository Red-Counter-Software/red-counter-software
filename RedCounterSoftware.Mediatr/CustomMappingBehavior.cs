using MediatR;
using RedCounterSoftware.Common.Validation;

namespace RedCounterSoftware.Mediatr;

public class CustomMappingBehavior<TDto, TEntity>(Func<TDto, TEntity> mappingFunc)
    : IPipelineBehavior<CrudRequestBase<TDto, TEntity>, Result<TEntity>>
    where TDto : class
    where TEntity : class, new()
{
    private readonly Func<TDto, TEntity> mappingFunc = mappingFunc ?? throw new ArgumentNullException(nameof(mappingFunc), "mappingFunc is required");

    public async Task<Result<TEntity>> Handle(CrudRequestBase<TDto, TEntity> request, RequestHandlerDelegate<Result<TEntity>> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        request.Entity = this.mappingFunc(request.Dto);

        return await next().ConfigureAwait(false);
    }
}
