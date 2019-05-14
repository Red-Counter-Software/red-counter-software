namespace RedCounterSoftware.Common
{
    public interface IDataObject
    {
        object Id { get; }

        string FormatId();
    }
}
