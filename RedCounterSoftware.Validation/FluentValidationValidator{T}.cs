namespace RedCounterSoftware.Validation
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Common.Extensions;
    using Common.Validation;
    using FluentValidation;

    public abstract class FluentValidationValidator<T> : AbstractValidator<T>, ICustomValidator<T>
        where T : class
    {
        protected FluentValidationValidator() => this.CascadeMode = CascadeMode.StopOnFirstFailure;

        public async Task<Result<T>> PerformValidation(T toValidate, int? index = null)
        {
            if (toValidate == null)
            {
                throw new ArgumentNullException(nameof(toValidate));
            }

            var result = await this.ValidateAsync(toValidate);
            return new Result<T>(toValidate, result.Errors.Select(c => new Failure(c.PropertyName, c.ErrorMessage, c.AttemptedValue ?? string.Empty)).ToList(), index);
        }

        public Task<Result<T>> ValidateProperty<TK>(T toValidate, Expression<Func<T, TK>> propertySelector)
        {
            if (toValidate == null)
            {
                throw new ArgumentNullException(nameof(toValidate));
            }

            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            var result = this.Validate(toValidate);
            return Task.FromResult(new Result<T>(toValidate, result.Errors.Where(e => e.PropertyName.StartsWith(propertySelector.GetPropertyName())).Select(c => new Failure(c.PropertyName, c.ErrorMessage, c.AttemptedValue ?? string.Empty))
                    .ToList()));
        }
    }
}
