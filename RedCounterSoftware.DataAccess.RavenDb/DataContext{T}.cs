namespace RedCounterSoftware.DataAccess.RavenDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Omu.ValueInjecter;

    using Raven.Client.Documents;
    using Raven.Client.Documents.Indexes;
    using Raven.Client.Documents.Linq;
    using Raven.Client.Documents.Session;

    using RedCounterSoftware.Common;
    using RedCounterSoftware.Common.Extensions;
    using RedCounterSoftware.DataAccess.RavenDb.Extensions;

    public abstract class DataContext<T> : IDataContext<T>
    where T : RecordBase
    {
        private bool disposedValue; // To detect redundant calls

        protected DataContext(IDocumentStore store)
        {
            this.Store = store ?? throw new ArgumentNullException(nameof(store));
            this.Session = store.OpenAsyncSession();
        }

        protected IDocumentStore Store { get; }

        protected IAsyncDocumentSession Session { get; }

        public Task<int> Count(CancellationToken cancellationToken = default) => this.Session.Query<T>().CountAsync(cancellationToken);

        public Task<bool> ExistsBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var exp = selector.Body.CreateKeyComparisonExpression(Expression.Constant(value));
            var lambda = (Expression<Func<T, bool>>)Expression.Lambda(exp, false, selector.GetParameterExpression());
            return this.Session.Query<T>().AnyAsync(lambda, cancellationToken);
        }

        public Task<T> GetBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var exp = selector.Body.CreateKeyComparisonExpression(Expression.Constant(value));
            var lambda = (Expression<Func<T, bool>>)Expression.Lambda(exp, false, selector.GetParameterExpression());
            return this.Session.Query<T>().SingleOrDefaultAsync(lambda, cancellationToken);
        }

        public virtual async Task<SearchResult<T>> GetByMultipleValues<TK>(Expression<Func<T, TK>> selector, TK[] values, CancellationToken cancellationToken = default)
        {
            if (values == null || values.Length == 0)
            {
                return new SearchResult<T>(0, new List<T>());
            }

            var filter = selector.InExpression(values);

            var data = await this.Session.Query<T>().Where(filter).ToListAsync(cancellationToken).ConfigureAwait(false);

            return new SearchResult<T>(data.Count, data);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<T> Add<TId>(Expression<Func<T, TId>> filter, TId id, T toAdd, CancellationToken cancellationToken = default)
        {
            if (toAdd == null)
            {
                throw new ArgumentNullException(nameof(toAdd));
            }

            await this.Session.StoreAsync(toAdd, cancellationToken).ConfigureAwait(false);
            await this.Session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return toAdd;
        }

        public async Task<T[]> AddBulk<TId>(Expression<Func<T, TId>> filter, T[] toAdd, CancellationToken cancellationToken = default)
        {
            if (toAdd == null)
            {
                throw new ArgumentNullException(nameof(toAdd));
            }

            using var bulkInsert = this.Store.BulkInsert(token: cancellationToken);
            foreach (var item in toAdd)
            {
                _ = await bulkInsert.StoreAsync(item).ConfigureAwait(false);
            }

            return toAdd;
        }

        public async Task Delete<TId>(Expression<Func<T, TId>> filter, TId id, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            this.Session.Delete(id);
            await this.Session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> Patch<TId, TK>(Expression<Func<T, TId>> filter, TId id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            this.Session.Advanced.Patch(id.ToString(), selector, value);
            await this.Session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            var result = await this.Session.LoadAsync<T>(id.ToString(), cancellationToken).ConfigureAwait(false);

            this.Session.Advanced.Evict(result);

            return result;
        }

        public virtual Task<SearchResult<T>> Search(SearchParameters<T> searchParameters, CancellationToken cancellationToken = default)
        {
            if (searchParameters == null)
            {
                throw new ArgumentNullException(nameof(searchParameters));
            }

            var query = this.ComposeSearch(searchParameters);
            return this.SearchFilters(query, searchParameters, cancellationToken);
        }

        public virtual async Task<T> Update<TId>(T toUpdate, TId id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var item = await this.Session.LoadAsync<T>(id.ToString(), cancellationToken).ConfigureAwait(false);
            _ = item.InjectFrom(toUpdate);
            await this.Session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return item;
        }

        protected abstract IRavenQueryable<T> ComposeSearch(SearchParameters<T> searchParameters);

        protected virtual async Task<SearchResult<TSearch>> SearchFilters<TSearch>(IRavenQueryable<TSearch> queryable, SearchParameters<TSearch> searchParameters, CancellationToken cancellationToken)
        {
            if (searchParameters == null)
            {
                throw new ArgumentNullException(nameof(searchParameters));
            }

            var ordered = searchParameters.IsDescending ? queryable.OrderByDescending(searchParameters.SortExpression) : queryable.OrderBy(searchParameters.SortExpression);
            var paged = ordered.Skip(searchParameters.PageSize * searchParameters.CurrentPage).Take(searchParameters.PageSize);
            var count = await queryable.CountAsync(cancellationToken).ConfigureAwait(false);
            var result = await paged.ToListAsync(cancellationToken).ConfigureAwait(false);
            return new SearchResult<TSearch>(count, result);
        }

        protected IRavenQueryable<TSearch> QueryWithIndex<TSearch, TIndex>(SearchParameters<TSearch> searchParameters, params Expression<Func<TSearch, object>>[] filters)
            where TIndex : AbstractIndexCreationTask<T, TSearch>, new()
        {
            if (searchParameters == null)
            {
                throw new ArgumentNullException(nameof(searchParameters));
            }

            var query = this.Session.Query<TSearch, TIndex>();

            query = filters.Aggregate(query, (current, criteria) => current.Search(criteria, searchParameters.SearchTerm));

            return query;
        }

        protected Task<SearchResult<TSearch>> SearchWithIndex<TSearch, TIndex>(SearchParameters<TSearch> searchParameters, CancellationToken cancellationToken, params Expression<Func<TSearch, object>>[] filters)
            where TIndex : AbstractIndexCreationTask<T, TSearch>, new()
        {
            if (searchParameters == null)
            {
                throw new ArgumentNullException(nameof(searchParameters));
            }

            var query = this.QueryWithIndex<TSearch, TIndex>(searchParameters, filters);

            return this.SearchFilters(query, searchParameters, cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Session.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.disposedValue = true;
            }
        }
    }
}
