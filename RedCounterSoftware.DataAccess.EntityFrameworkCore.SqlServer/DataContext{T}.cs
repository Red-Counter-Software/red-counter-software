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

    public abstract class DataContext<T> : IDataContext<T>
        where T : class
    {
        private readonly DbSet<T> entitySet;

        private bool disposedValue; // To detect redundant calls

        protected DataContext(DbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.entitySet = this.Context.Set<T>();
        }

        protected DbContext Context { get; }

        public virtual async Task<T> Add<TId>(Expression<Func<T, TId>> filter, TId id, T toAdd, CancellationToken cancellationToken = default)
        {
            var result = await this.entitySet.AddAsync(toAdd, cancellationToken).ConfigureAwait(false);
            _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return result.Entity;
        }

        public async Task<T[]> AddBulk<TId>(Expression<Func<T, TId>> filter, T[] toAdd, CancellationToken cancellationToken = default)
        {
            if (toAdd == null)
            {
                throw new ArgumentNullException(nameof(toAdd));
            }

            var results = new List<T>();
            foreach (var item in toAdd)
            {
                var result = await this.entitySet.AddAsync(item, cancellationToken).ConfigureAwait(false);
                results.Add(result.Entity);
            }

            _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return results.ToArray();
        }

        public virtual Task<int> Count(CancellationToken cancellationToken = default)
        {
            return this.entitySet.CountAsync(cancellationToken);
        }

        public virtual async Task Delete<TId>(Expression<Func<T, TId>> filter, TId id, CancellationToken cancellationToken = default)
        {
            var lambda = filter.GetFilterExpression(id);
            var entity = await this.entitySet.SingleOrDefaultAsync(lambda, cancellationToken).ConfigureAwait(false);
            if (entity != null)
            {
                _ = this.entitySet.Remove(entity);
                _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual Task<bool> ExistsBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var lambda = selector.GetFilterExpression(value);
            return this.entitySet.AnyAsync(lambda, cancellationToken);
        }

        public virtual Task<T> GetBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var lambda = selector.GetFilterExpression(value);
            return this.entitySet.SingleOrDefaultAsync(lambda, cancellationToken);
        }

        public virtual async Task<SearchResult<T>> GetByMultipleValues<TK>(Expression<Func<T, TK>> selector, TK[] values, CancellationToken cancellationToken = default)
        {
            if (values == null || values.Length == 0)
            {
                return new SearchResult<T>(0, new List<T>());
            }

            var filter = selector.InExpression(values);

            var data = await this.entitySet.Where(filter).ToListAsync(cancellationToken).ConfigureAwait(false);

            return new SearchResult<T>(data.Count, data);
        }

        public virtual async Task<T> Patch<TId, TK>(Expression<Func<T, TId>> filter, TId id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var lambda = filter.GetFilterExpression(id);
            var entity = await this.entitySet.SingleOrDefaultAsync(lambda, cancellationToken).ConfigureAwait(false);

            if (entity == null)
            {
                throw new InvalidOperationException($"Entity with id {id} was not found.");
            }

            var propertyName = selector.GetPropertyName();
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
            typeof(T).GetProperty(propertyName)!.SetValue(entity, value);
#pragma warning restore SA1009 // Closing parenthesis should be spaced correctly
            _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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
            GC.SuppressFinalize(this);
        }

        protected abstract IQueryable<T> ComposeSearch(SearchParameters<T> searchParameters);

        protected virtual SearchResult<T> SearchFilters(IQueryable<T> queryable, SearchParameters<T> searchParameters)
        {
            if (searchParameters == null)
            {
                throw new ArgumentNullException(nameof(searchParameters));
            }

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
                    this.Context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.disposedValue = true;
            }
        }
    }
}
