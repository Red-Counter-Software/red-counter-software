namespace RedCounterSoftware.Common.Account
{
    using System.Collections.ObjectModel;

    public interface IRole
    {
        /// <summary>
        /// Gets the user friendly name of the role.
        /// </summary>
        /// <value>
        /// The user friendly name of the role.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the list of associated claims.
        /// </summary>
        /// <value>
        /// The list of associated claims.
        /// </value>
        Collection<string> Claims { get; }
    }
}
