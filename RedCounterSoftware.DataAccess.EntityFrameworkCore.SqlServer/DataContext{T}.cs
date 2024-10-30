namespace RedCounterSoftware.DataAccess.EntityFrameworkCore.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using RedCounterSoftware.Common;
    using RedCounterSoftware.Common.Extensions;

    public abstract class DataContext<T> : ReadOnlyDataContext<T>, IDataContext<T>
        where T : class
    {
        private readonly DbSet<T> entitySet;

        protected DataContext(DbContext context)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(context);

            this.entitySet = context.Set<T>();
        }

        public virtual async Task<T> Add<TId>(Expression<Func<T, TId>> filter, TId id, T toAdd, CancellationToken cancellationToken = default)
        {
            _ = await this.entitySet.AddAsync(toAdd, cancellationToken).ConfigureAwait(false);
            _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var item = await this.GetBy(filter, id, cancellationToken).ConfigureAwait(false);

            return item!;
        }

        public async Task<T[]> AddBulk<TId>(Expression<Func<T, TId>> filter, T[] toAdd, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(toAdd);

            var results = new List<T>();
            foreach (var item in toAdd)
            {
                var result = await this.entitySet.AddAsync(item, cancellationToken).ConfigureAwait(false);
                results.Add(result.Entity);
            }

            _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return [.. results];
        }

        public virtual async Task Delete<TId>(Expression<Func<T, TId>> filter, TId id, bool hardDelete = false, CancellationToken cancellationToken = default)
        {
            var lambda = filter.GetFilterExpression(id);
            var entity = await this.entitySet.SingleOrDefaultAsync(lambda, cancellationToken).ConfigureAwait(false);
            if (entity != null)
            {
                if (hardDelete || typeof(T).GetInterface(typeof(IDeletable).Name) == null)
                {
                    _ = this.entitySet.Remove(entity);
                }
                else
                {
                    ((IDeletable)entity).IsDeleted = true;
                }

                _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task<T> Patch<TId, TK>(Expression<Func<T, TId>> filter, TId id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(filter);

            var lambda = filter.GetFilterExpression(id);
            var entity = await this.GetEntitySet().SingleOrDefaultAsync(lambda, cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException($"Entity with id {id} was not found.");
            var propertyName = selector.GetPropertyName();

            typeof(T).GetProperty(propertyName)!.SetValue(entity, value);

            _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return entity;
        }

        public virtual async Task<T> Update<TId>(T toUpdate, TId id, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(toUpdate);

            _ = this.Context.Update(toUpdate);
            _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return toUpdate;
        }
    }
}
