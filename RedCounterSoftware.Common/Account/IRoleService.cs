namespace RedCounterSoftware.Common.Account
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Store service to interact with <see cref="T:RedCounterSoftware.Common.Account.IRole" /> dataobjects.
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Attempts to find the <see cref="Role"/>s owned by the <see cref="User"/> with provided <see cref="userId"/>. Returns an empty collection if the <see cref="User"/> is not found or if it has no <see cref="Role"/> collection associated, otherwise the collection is returned.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the collection of roles of the specified user or null if not found.</returns>
        Task<List<IRole>> GetByUserId(object userId, CancellationToken cancellationToken = default);
    }
}
