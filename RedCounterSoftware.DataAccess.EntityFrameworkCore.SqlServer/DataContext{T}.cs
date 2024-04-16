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
        where T : RecordBase
    {
        private readonly DbSet<T> entitySet;

        protected DataContext(DbContext context)
            : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

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

        public virtual async Task<T> Patch<TId, TK>(Expression<Func<T, TId>> filter, TId id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var lambda = filter.GetFilterExpression(id);
            var entity = await this.GetEntitySet().SingleOrDefaultAsync(lambda, cancellationToken).ConfigureAwait(false);

            if (entity == null)
            {
                throw new InvalidOperationException($"Entity with id {id} was not found.");
            }

            var propertyName = selector.GetPropertyName();

            typeof(T).GetProperty(propertyName)!.SetValue(entity, value);

            _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return entity;
        }

        public virtual async Task<T> Update<TId>(T toUpdate, TId id, CancellationToken cancellationToken = default)
        {
            if (toUpdate == null)
            {
                throw new ArgumentNullException(nameof(toUpdate));
            }

            _ = this.Context.Update(toUpdate);
            _ = await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return toUpdate;
        }
    }
}
