namespace RedCounterSoftware.Common.Account
{
    using System;

    public interface IUser
    {
        object Id { get; }

        Guid? ActivationGuid { get; }

        string Email { get; }

        byte[] Password { get; }

        Guid? PasswordResetGuid { get; }

        object PersonId { get; }
    }
}
