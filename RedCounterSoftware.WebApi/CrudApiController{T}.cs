namespace RedCounterSoftware.WebApi
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RedCounterSoftware.Common;
    using RedCounterSoftware.Common.Extensions;
    using RedCounterSoftware.Common.Logging;
    using RedCounterSoftware.Common.Validation;
    using RedCounterSoftware.Logging.Web;

    [ApiController]
    public abstract class CrudApiController<T> : ControllerBase
        where T : class
    {
        private readonly ILogger logger;

        protected CrudApiController(IStoreService<T> storeService, ILogger logger)
        {
            this.StoreService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected IStoreService<T> StoreService { get; }

        protected async Task<Result<T>> Add<TK>(Expression<Func<T, TK>> filter, TK id, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using (this.logger.BeginScope(LoggingEvents.Crud))
            using (this.logger.GetCommonScopes(this.HttpContext, this.HttpContext.User))
            {
                var typeName = typeof(T).Name;

                this.logger.LogInformation(LoggingEvents.CrudAdd, "Attempting to add {itemType} {itemData}", typeName, item.ToString());

                var result = await this.StoreService.Add(filter, id, item);
                if (!result.IsValid)
                {
                    this.logger.LogInformation(LoggingEvents.CrudAdd, $"Validation errors occurred while attempting to add {{itemType}} {{itemData}}:{Environment.NewLine}{{errors}}", typeName, item.ToString(), result.FormatFailuresForLog());
                }
                else
                {
                    this.logger.LogInformation(LoggingEvents.CrudAdd, "{typeName} {typeData} added successfully", typeName, item.ToString());
                }

                return result.ToCamelCasedPropertiesResult();
            }
        }

        protected virtual Task<T> GetById<TK>(Expression<Func<T, TK>> filter, TK id) => this.StoreService.GetBy(filter, id);

        protected virtual Task<Result> Delete<TK>(Expression<Func<T, TK>> filter, TK id) => this.StoreService.Delete(filter, id);

        protected virtual async Task<Result<T>> Patch<TK>(Expression<Func<T, TK>> filter, TK id, string propertyName, object value)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Cannot be empty", nameof(propertyName));
            }

            var exp = propertyName.GetPropertyExpression<T>();
            var type = Nullable.GetUnderlyingType(exp.GetPropertyType()) ?? exp.GetPropertyType();
            object changedValue;
            if (type.IsEnum)
            {
                changedValue = Enum.Parse(type, value.ToString());
            }
            else
            {
                changedValue = (value == null) ? null : Convert.ChangeType(value, type);
            }

            using (this.logger.BeginScope(LoggingEvents.Crud))
            using (this.logger.GetCommonScopes(this.HttpContext, this.HttpContext.User))
            {
                this.logger.LogInformation("Updating property \"{property}\" on object \"{object}\" with id \"{id}\" using value \"{value}\"", propertyName, nameof(T), id, value);

                var result = await this.StoreService.Patch(filter, id, exp, changedValue);
                return result.ToCamelCasedPropertiesResult();
            }
        }

        protected virtual async Task<int> Count()
        {
            var count = await this.StoreService.Count();
            return count;
        }

        protected virtual Task<SearchResult<T>> Search(SearchParameters<T> searchParameters) => this.StoreService.Search(searchParameters);
    }
}
