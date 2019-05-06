namespace RedCounterSoftware.Common.Validation
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface ICustomValidator<T>
        where T : class
    {
        Task<Result<T>> PerformValidation(T toValidate, int? index = null);

        Task<Result<T>> ValidateProperty<TK>(T toValidate, Expression<Func<T, TK>> propertySelector);
    }
}
