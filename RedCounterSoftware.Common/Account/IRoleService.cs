namespace RedCounterSoftware.Common.Account
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Store service to interact with <see cref="IRole" /> dataobjects.
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Attempts to find the <see cref="IRole"/>s owned by the <see cref="IUser"/> with provided <see cref="userId"/>. Returns an empty collection if the <see cref="IUser"/> is not found or if it has no <see cref="IRole"/> collection associated, otherwise the collection is returned.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the collection of roles of the specified user or null if not found.</returns>
        Task<List<IRole>> GetByUserId(object userId, CancellationToken cancellationToken = default);
    }
}
