using MediatR;
using RedCounterSoftware.Common;

namespace RedCounterSoftware.Mediatr;

public class DeleteEntityRequest<TEntity, TId>(TId id, bool hardDelete = false) : IRequest
    where TEntity : class, IIdentifiable<TId>
{
    public TId Id { get; } = id ?? throw new ArgumentNullException(nameof(id), "id is required");

    public bool HardDelete { get; } = hardDelete;
}
