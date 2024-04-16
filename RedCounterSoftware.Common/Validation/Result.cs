namespace RedCounterSoftware.Common.Validation
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Extensions;

    public class Result(Collection<Failure> failures, int? index = null)
    {
        public int? Index { get; } = index;

        public bool IsValid => this.Failures.Count == 0;

        public Collection<Failure> Failures { get; } = failures ?? throw new ArgumentNullException(nameof(failures));

        public string FormatFailuresForLog()
        {
            return this.Failures
                .Select(c => $"{c.PropertyName}: {c.ErrorMessage} - Attempted value: {c.AttemptedValue ?? string.Empty}")
                .DefaultIfEmpty(string.Empty)
                .Aggregate((s1, s2) => s1 + Environment.NewLine + s2);
        }

        public Result ToCamelCasedPropertiesResult()
        {
            return new Result(new Collection<Failure>(this.Failures.Select(f => new Failure(f.PropertyName.ToCamelCase(), f.ErrorMessage, f.AttemptedValue)).ToList()));
        }
    }
}
