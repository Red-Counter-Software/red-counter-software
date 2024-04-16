namespace RedCounterSoftware.Common.Validation
{
    using System;

    public class Failure(string propertyName, string errorMessage)
    {
        public Failure(string propertyName, string errorMessage, object attemptedValue)
            : this(propertyName, errorMessage) => this.AttemptedValue = attemptedValue;

        public string PropertyName { get; } = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

        public string ErrorMessage { get; } = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));

        public object AttemptedValue { get; } = string.Empty;
    }
}
