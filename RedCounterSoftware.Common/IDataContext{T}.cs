namespace RedCounterSoftware.Common
{
    /// <inheritdoc />
    /// <summary>
    /// Generic DataContext interface to interact with underlying storage of a specific type <see cref="T" /> of entity.
    /// </summary>
    /// <typeparam name="T">The type of the entity handled.</typeparam>
    public interface IDataContext<T> : IReadDataContext<T>, IWriteDataContext<T>
        where T : RecordBase
    {
    }
}
