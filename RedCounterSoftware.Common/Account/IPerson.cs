namespace RedCounterSoftware.Common.Account
{
    using System;

    public interface IPerson
    {
        object Id { get; }

        DateTimeOffset BirthDate { get; }

        string FirstName { get; }

        string LastName { get; }
    }
}
