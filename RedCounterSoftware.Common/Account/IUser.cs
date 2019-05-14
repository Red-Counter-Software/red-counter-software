namespace RedCounterSoftware.Common.Account
{
    using System;

    public interface IUser : IDataObject
    {
        Guid? ActivationGuid { get; }

        string Email { get; }

        Guid? PasswordResetGuid { get; }

        object PersonId { get; }
    }
}
