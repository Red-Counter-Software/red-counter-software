namespace RedCounterSoftware.Common.Validation
{
    using System;

    public class Failure
    {
        public Failure(string propertyName, string errorMessage)
        {
            this.PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            this.ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            this.AttemptedValue = string.Empty;
        }

        public Failure(string propertyName, string errorMessage, object attemptedValue)
            : this(propertyName, errorMessage) => this.AttemptedValue = attemptedValue;

        public string PropertyName { get; }

        public string ErrorMessage { get; }

        public object AttemptedValue { get; }
    }
}
