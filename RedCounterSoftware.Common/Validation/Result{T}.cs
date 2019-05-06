namespace RedCounterSoftware.Common.Validation
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;

    public class Result<T> : Result
        where T : class
    {
        public Result(T item, List<Failure> failures, int? index = null)
            : base(failures, index) => this.Item = item;

        public T Item { get; }

        public new Result<T> ToCamelCasedPropertiesResult()
        {
            return new Result<T>(
                this.Item,
                this.Failures
                    .Select(f => new Failure(f.PropertyName.ToCamelCase(), f.ErrorMessage, f.AttemptedValue))
                    .ToList());
        }
    }
}
