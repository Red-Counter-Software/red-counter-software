namespace RedCounterSoftware.Common.Account
{
    using System;

    public interface IUser
    {
        object Id { get; }

        Guid? ActivationGuid { get; }

        string Email { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Entity Framework")]
        byte[] Password { get; }

        Guid? PasswordResetGuid { get; }

        object PersonId { get; }
    }
}
