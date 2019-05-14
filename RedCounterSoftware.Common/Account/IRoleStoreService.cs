namespace RedCounterSoftware.Common.Account
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <inheritdoc />
    /// <summary>
    /// Store service to interact with <see cref="T:RedCounterSoftware.Common.Account.IRole" /> dataobjects.
    /// </summary>
    public interface IRoleStoreService : IStoreService<IRole>
    {
        /// <summary>
        /// Attempts to find the <see cref="Role"/>s owned by the <see cref="User"/> with provided <see cref="userId"/>. Returns an empty collection if the <see cref="User"/> is not found or if it has no <see cref="Role"/> collection associated, otherwise the collection is returned.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the collection of roles of the specified user or null if not found.</returns>
        Task<List<IRole>> GetByUserId(object userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a search using the provided <see cref="SearchParameters{Role}"/>.
        /// </summary>
        /// <param name="searchParameters">The search parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="SearchResult{Role}"/> containing the entities matching the search criteria.</returns>
        Task<SearchResult<IRole>> Search(SearchParameters<IRole> searchParameters, CancellationToken cancellationToken = default);
    }
}
