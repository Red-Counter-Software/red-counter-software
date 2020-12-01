namespace RedCounterSoftware.Common.Validation
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Extensions;

    public class Result
    {
        public Result(Collection<Failure> failures, int? index = null)
        {
            this.Failures = failures ?? throw new ArgumentNullException(nameof(failures));
            this.Index = index;
        }

        public int? Index { get; }

        public bool IsValid => !this.Failures.Any();

        public Collection<Failure> Failures { get; }

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
