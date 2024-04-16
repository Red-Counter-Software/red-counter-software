namespace RedCounterSoftware.Common.Validation
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Extensions;

    public class Result<T>(T item, Collection<Failure> failures, int? index = null) : Result(failures, index)
        where T : class
    {
        public T Item { get; } = item;

        public new Result<T> ToCamelCasedPropertiesResult()
        {
            return new Result<T>(
                this.Item,
                new Collection<Failure>(this.Failures
                .Select(f => new Failure(f.PropertyName.ToCamelCase(), f.ErrorMessage, f.AttemptedValue))
                .ToList()));
        }
    }
}
