using MediatR;
using RedCounterSoftware.Common;
using RedCounterSoftware.Common.Extensions;
using RedCounterSoftware.Common.Validation;

namespace RedCounterSoftware.Mediatr;

public class ValidationBehavior<TDto, TEntity>(ICustomValidator<TEntity> validator)
    : IPipelineBehavior<CrudRequestBase<TDto, TEntity>, Result<TEntity>>
    where TDto : class
    where TEntity : class, IIdentifiable, new()
{
    private readonly ICustomValidator<TEntity> validator = validator ?? throw new ArgumentNullException(nameof(validator), "validator is required");

    public async Task<Result<TEntity>> Handle(CrudRequestBase<TDto, TEntity> request, RequestHandlerDelegate<Result<TEntity>> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        var validationResult = await this.validator.PerformValidation(request.Entity).ConfigureAwait(false);

        if (request.AdditionalValidation is not null)
        {
            var additionalFailures = await request.AdditionalValidation(request.Entity).ConfigureAwait(false);
            request.Result.Failures.AddRange(additionalFailures);
        }

        if (!validationResult.IsValid)
        {
            return validationResult;
        }

        // Request is valid, continue with the next handler
        request.Result = validationResult;

        return await next().ConfigureAwait(false);
    }
}
