namespace RedCounterSoftware.Common;

public interface IIdentifiable
{
    object Id { get; }
}

public interface IIdentifiable<out T> : IIdentifiable
{
    new T Id { get; }
}
