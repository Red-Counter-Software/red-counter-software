namespace RedCounterSoftware.Common.Account
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPersonService
    {
        Task<IPerson> GetById(object id, CancellationToken cancellationToken = default);
    }
}
