namespace RedCounterSoftware.Common.Account
{
    using System.Collections.Generic;

    public interface IRole : IDataObject
    {
        /// <summary>
        /// Gets the user friendly name of the role.
        /// </summary>
        /// <value>
        /// The user friendly name of the role.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the list of associated <see cref="Permissions"/>.
        /// </summary>
        /// <value>
        /// The list of associated <see cref="Permissions"/>.
        /// </value>
        List<string> Claims { get; }
    }
}
