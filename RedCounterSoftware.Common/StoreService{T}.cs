namespace RedCounterSoftware.Common
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Extensions;
    using Common.Validation;

    public abstract class StoreService<T> : IStoreService<T>
        where T : class
    {
        protected StoreService(IDataContext<T> context, ICustomValidator<T> validator)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        protected IDataContext<T> Context { get; }

        protected ICustomValidator<T> Validator { get; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual async Task<Result<T>> Add<TId>(Expression<Func<T, TId>> filter, TId id, T toAdd, CancellationToken cancellationToken = default)
        {
            if (toAdd == null)
            {
                throw new ArgumentNullException(nameof(toAdd));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await this.Validator.PerformValidation(toAdd).ConfigureAwait(false);
            var exists = await this.Context.ExistsBy(filter, id, cancellationToken).ConfigureAwait(false);

            if (exists)
            {
                result.Failures.Add(new Failure(nameof(id), $"{typeof(T).Name} with this id already exists", id));
            }

            await this.AddItemAdditionalChecks(toAdd, result, cancellationToken).ConfigureAwait(false);

            if (!result.IsValid)
            {
                return result;
            }

            var created = await this.Context.Add(filter, id, toAdd, cancellationToken).ConfigureAwait(false);
            return new Result<T>(created, new Collection<Failure>());
        }

        public virtual Task<int> Count(CancellationToken cancellationToken = default) => this.Context.Count(cancellationToken);

        public virtual async Task<Result> Delete<TId>(Expression<Func<T, TId>> filter, TId id, CancellationToken cancellationToken = default)
        {
            await this.Context.Delete(filter, id, cancellationToken).ConfigureAwait(false);

            return new Result(new Collection<Failure>());
        }

        public virtual Task<T> GetBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default) => this.Context.GetBy(selector, value, cancellationToken);

        public virtual Task<SearchResult<T>> GetByMultipleValues<TK>(Expression<Func<T, TK>> selector, TK[] values, CancellationToken cancellationToken = default)
        {
            return this.Context.GetByMultipleValues(selector, values, cancellationToken);
        }

        public virtual async Task<Result<T>> Patch<TId, TK>(Expression<Func<T, TId>> filter, TId id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var current = await this.Context.GetBy(filter, id, cancellationToken).ConfigureAwait(false);

            if (current == null)
            {
                throw new InvalidOperationException($"{typeof(T)} with id {id} was not found");
            }

            // Initialize empty result with unpatched item
            var result = new Result<T>(current, new Collection<Failure>());

            // Apply additional custom rules and get the potentially modified value. Result gets updated with failuers.
            var updatedValue = await this.PatchItemAdditionalChecks(current, selector, value, result, cancellationToken).ConfigureAwait(false);

            // Update the item with the new value
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
            typeof(T).GetProperty(selector.GetPropertyName())!.SetValue(current, updatedValue);
#pragma warning restore SA1009 // Closing parenthesis should be spaced correctly

            // Validate the new item with the new value
            var propertyResult = await this.Validator.ValidateProperty(current, selector).ConfigureAwait(false);

            // Add failures obtained by the additional checks
            propertyResult.Failures.AddRange(result.Failures);

            if (!propertyResult.IsValid)
            {
                return propertyResult;
            }

            var patched = await this.Context.Patch(filter, id, selector, updatedValue, cancellationToken).ConfigureAwait(false);
            return new Result<T>(patched, new Collection<Failure>());
        }

        public Task<SearchResult<T>> Search(SearchParameters<T> searchParameters, CancellationToken cancellationToken = default)
        {
            return this.Context.Search(searchParameters, cancellationToken);
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
