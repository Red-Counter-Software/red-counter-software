namespace RedCounterSoftware.Common;

public interface IEntity : IDeletable, IIdentifiable
{
}

public interface IEntity<TId> : IEntity, IIdentifiable<TId>
{
}