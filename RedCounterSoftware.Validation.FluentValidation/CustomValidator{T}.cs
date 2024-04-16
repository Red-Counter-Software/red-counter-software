#pragma warning disable SA1200 // Using directives should be placed correctly
using FluentValidation;
#pragma warning restore SA1200 // Using directives should be placed correctly

namespace RedCounterSoftware.Validation.FluentValidation
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Common.Extensions;
    using Common.Validation;

    public abstract class CustomValidator<T> : AbstractValidator<T>, ICustomValidator<T>
        where T : class
    {
        protected CustomValidator() => this.ClassLevelCascadeMode = CascadeMode.Continue;

        public async Task<Result<T>> PerformValidation(T toValidate, int? index = null)
        {
            ArgumentNullException.ThrowIfNull(toValidate);

            var result = await this.ValidateAsync(toValidate).ConfigureAwait(false);
            return new Result<T>(toValidate, new Collection<Failure>(result.Errors.Select(c => new Failure(c.PropertyName, c.ErrorMessage, c.AttemptedValue ?? string.Empty)).ToList()), index);
        }

        public Task<Result<T>> ValidateProperty<TK>(T toValidate, Expression<Func<T, TK>> propertySelector)
        {
            ArgumentNullException.ThrowIfNull(toValidate);
            ArgumentNullException.ThrowIfNull(propertySelector);

            var result = this.Validate(toValidate);
            return Task.FromResult(new Result<T>(toValidate, new Collection<Failure>(result.Errors.Where(e => e.PropertyName.StartsWith(propertySelector.GetPropertyName())).Select(c => new Failure(c.PropertyName, c.ErrorMessage, c.AttemptedValue ?? string.Empty))
                    .ToList())));
        }
    }
}
