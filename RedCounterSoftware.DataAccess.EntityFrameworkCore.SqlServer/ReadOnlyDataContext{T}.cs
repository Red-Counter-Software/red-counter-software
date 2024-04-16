namespace RedCounterSoftware.DataAccess.EntityFrameworkCore.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using RedCounterSoftware.Common;
    using RedCounterSoftware.Common.Extensions;

    public abstract class ReadOnlyDataContext<T> : IReadDataContext<T>
        where T : RecordBase
    {
        private readonly DbSet<T> entitySet;

        private bool disposedValue; // To detect redundant calls

        protected ReadOnlyDataContext(DbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.entitySet = this.Context.Set<T>();
        }

        protected DbContext Context { get; }

        public virtual Task<int> Count(CancellationToken cancellationToken = default)
        {
            return this.entitySet.CountAsync(cancellationToken);
        }

        public virtual Task<bool> ExistsBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var lambda = selector.GetFilterExpression(value);
            return this.entitySet.AnyAsync(lambda, cancellationToken);
        }

        public virtual Task<T?> GetBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var lambda = selector.GetFilterExpression(value);
            return this.GetEntitySet().SingleOrDefaultAsync(lambda, cancellationToken);
        }

        public virtual async Task<SearchResult<T>> GetByMultipleValues<TK>(Expression<Func<T, TK>> selector, TK[] values, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (values == null || values.Length == 0)
            {
                return new SearchResult<T>(0, new List<T>());
            }

            var filter = selector.InExpression(values);

            var data = await this.GetEntitySet().Where(filter).ToListAsync(cancellationToken).ConfigureAwait(false);

            return new SearchResult<T>(data.Count, data);
        }

        public virtual Task<SearchResult<T>> Search(SearchParameters<T> searchParameters, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(searchParameters);

            var query = this.ComposeSearch(searchParameters);
            return this.SearchFilters(query, searchParameters, cancellationToken);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);

            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.disposedValue = true;
            }
        }

        protected abstract IQueryable<T> ComposeSearch(SearchParameters<T> searchParameters);

        protected virtual async Task<SearchResult<T>> SearchFilters(IQueryable<T> queryable, SearchParameters<T> searchParameters, CancellationToken cancellationToken)
        {
            if (searchParameters == null)
            {
                throw new ArgumentNullException(nameof(searchParameters));
            }

            var ordered = searchParameters.IsDescending ? queryable.OrderByDescending(searchParameters.SortExpression) : queryable.OrderBy(searchParameters.SortExpression);
            var paged = ordered.Skip(searchParameters.PageSize * searchParameters.CurrentPage).Take(searchParameters.PageSize);
            var count = await queryable.CountAsync(cancellationToken).ConfigureAwait(false);
            var result = await paged.ToListAsync(cancellationToken).ConfigureAwait(false);
            return new SearchResult<T>(count, result);
        }

        protected virtual IQueryable<T> GetEntitySet()
        {
            return this.entitySet;
        }
    }
}
