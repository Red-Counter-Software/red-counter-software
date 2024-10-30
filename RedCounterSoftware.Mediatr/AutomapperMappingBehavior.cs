using AutoMapper;
using MediatR;
using RedCounterSoftware.Common.Validation;

namespace RedCounterSoftware.Mediatr;

public class AutomapperMappingBehavior<TDto, TEntity>(IMapper mapper)
    : IPipelineBehavior<CrudRequestBase<TDto, TEntity>, Result<TEntity>>
    where TDto : class
    where TEntity : class, new()
{
    private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "mapper is required");

    public async Task<Result<TEntity>> Handle(CrudRequestBase<TDto, TEntity> request, RequestHandlerDelegate<Result<TEntity>> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        request.Entity = this.mapper.Map<TEntity>(request.Dto);

        return await next().ConfigureAwait(false);
    }
}