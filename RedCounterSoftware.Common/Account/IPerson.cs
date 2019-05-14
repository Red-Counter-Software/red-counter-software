namespace RedCounterSoftware.Common.Account
{
    using System;

    public interface IPerson : IDataObject
    {
        DateTimeOffset BirthDate { get; }
    }
}
