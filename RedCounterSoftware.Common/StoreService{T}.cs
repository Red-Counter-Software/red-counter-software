namespace RedCounterSoftware.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Extensions;
    using Common.Validation;

    public abstract class StoreService<T> : IStoreService<T>
        where T : class, IDataObject
    {
        protected StoreService(IDataContext<T> context, ICustomValidator<T> validator)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        protected IDataContext<T> Context { get; }

        protected ICustomValidator<T> Validator { get; }

        protected virtual Func<T, string> IdGenerationFunc => x => x.FormatId();

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual async Task<Result<T>> Add(T toAdd, CancellationToken cancellationToken = default)
        {
            if (toAdd == null)
            {
                throw new ArgumentNullException(nameof(toAdd));
            }

            var result = await this.Validator.PerformValidation(toAdd);
            var id = this.IdGenerationFunc(toAdd);
            var exists = await this.Context.ExistsBy(c => c.Id, id, cancellationToken);

            if (exists)
            {
                result.Failures.Add(new Failure(nameof(toAdd.Id), $"{nameof(T)} already exists", id));
            }

            await this.AddItemAdditionalChecks(toAdd, result, cancellationToken);

            if (!result.IsValid)
            {
                return result;
            }

            var created = await this.Context.Add(toAdd, cancellationToken);
            return new Result<T>(created, new List<Failure>());
        }

        public virtual Task<int> Count(CancellationToken cancellationToken = default) => this.Context.Count(cancellationToken);

        public virtual async Task<Result> Delete(object id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            await this.Context.Delete(id, cancellationToken);

            return new Result(new List<Failure>());
        }

        public virtual Task<T> GetBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default) => this.Context.GetBy(selector, value, cancellationToken);

        public virtual async Task<Result<T>> Patch<TK>(object id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            var current = await this.Context.GetBy(c => c.Id, id, cancellationToken);

            if (current == null)
            {
                throw new InvalidOperationException($"{typeof(T)} with id {id} was not found");
            }

            // Initialize empty result with unpatched item
            var result = new Result<T>(current, new List<Failure>());

            // Apply additional custom rules and get the potentially modified value. Result gets updated with failuers.
            var updatedValue = await this.PatchItemAdditionalChecks(current, selector, value, result, cancellationToken);

            // Get the new item with the new value
            var updated = current.With(selector, updatedValue);

            // Validate the new item with the new value
            var propertyResult = await this.Validator.ValidateProperty(updated, selector);

            // Add failures obtained by the additional checks
            propertyResult.Failures.AddRange(result.Failures);

            if (!propertyResult.IsValid)
            {
                return propertyResult;
            }

            var patched = await this.Context.Patch(id, selector, updatedValue, cancellationToken);
            return new Result<T>(patched, new List<Failure>());
        }

        protected abstract Task AddItemAdditionalChecks(T toAdd, Result<T> partialResult, CancellationToken cancellationToken = default);

        protected abstract Task<TK> PatchItemAdditionalChecks<TK>(T toPatch, Expression<Func<T, TK>> selector, TK value, Result<T> partialResult, CancellationToken cancellationToken = default);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.Context?.Dispose();
        }
    }
}
