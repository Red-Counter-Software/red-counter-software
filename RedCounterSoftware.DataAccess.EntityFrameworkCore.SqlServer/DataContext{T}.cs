namespace RedCounterSoftware.DataAccess.EntityFrameworkCore.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using RedCounterSoftware.Common;
    using RedCounterSoftware.Common.Extensions;

    public abstract class DataContext<T> : IDataContext<T>
        where T : class
    {
        private readonly DbContext context;

        private readonly DbSet<T> entitySet;

        private bool disposedValue = false; // To detect redundant calls

        public DataContext(DbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.entitySet = this.context.Set<T>();
        }

        public async Task<T> Add<TId>(Expression<Func<T, TId>> filter, TId id, T toAdd, CancellationToken cancellationToken = default)
        {
            var result = await this.entitySet.AddAsync(toAdd, cancellationToken);
            await this.context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public Task<int> Count(CancellationToken cancellationToken = default)
        {
            return this.entitySet.CountAsync(cancellationToken);
        }

        public async Task Delete<TId>(Expression<Func<T, TId>> filter, TId id, CancellationToken cancellationToken = default)
        {
            var lambda = filter.GetFilterExpression(id);
            var entity = await this.entitySet.SingleOrDefaultAsync(lambda, cancellationToken);
            if (entity != null)
            {
                this.entitySet.Remove(entity);
                await this.context.SaveChangesAsync(cancellationToken);
            }
        }

        public Task<bool> ExistsBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var lambda = selector.GetFilterExpression(value);
            return this.entitySet.AnyAsync(lambda, cancellationToken);
        }

        public Task<T> GetBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var lambda = selector.GetFilterExpression(value);
            return this.entitySet.SingleOrDefaultAsync(lambda, cancellationToken);
        }

        public async Task<SearchResult<T>> GetByMultipleValues<TK>(Expression<Func<T, TK>> selector, TK[] values, CancellationToken cancellationToken = default)
        {
            if (values == null || values.Length == 0)
            {
                return new SearchResult<T>(0, new List<T>());
            }

            var filter = selector.InExpression(values);

            var data = await this.entitySet.Where(filter).ToListAsync(cancellationToken);

            return new SearchResult<T>(data.Count, data);
        }

        public async Task<T> Patch<TId, TK>(Expression<Func<T, TId>> filter, TId id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return null;
            }

            var lambda = selector.GetFilterExpression(value);
            var entity = await this.entitySet.SingleOrDefaultAsync(lambda, cancellationToken);

            if (entity == null)
            {
                return null;
            }

            var propertyName = selector.GetPropertyName();
            typeof(T).GetProperty(propertyName, BindingFlags.Public).SetValue(entity, value);
            await this.context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public virtual Task<SearchResult<T>> Search(SearchParameters<T> searchParameters, CancellationToken cancellationToken = default)
        {
            var query = this.ComposeSearch(searchParameters);
            return Task.FromResult(this.SearchFilters(query, searchParameters));
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);

            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        protected abstract IQueryable<T> ComposeSearch(SearchParameters<T> searchParameters);

        protected virtual SearchResult<T> SearchFilters(IQueryable<T> queryable, SearchParameters<T> searchParameters)
        {
            var ordered = searchParameters.IsDescending ? queryable.OrderByDescending(searchParameters.SortExpression) : queryable.OrderBy(searchParameters.SortExpression);
            var paged = ordered.Skip(searchParameters.PageSize * searchParameters.CurrentPage).Take(searchParameters.PageSize);
            return new SearchResult<T>(queryable.Count(), paged.ToList());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.disposedValue = true;
            }
        }
    }
}
