namespace RedCounterSoftware.WebApi
{
    using System;
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
        where T : class, IDataObject
    {
        private readonly ILogger logger;

        protected CrudApiController(IStoreService<T> storeService, ILogger logger)
        {
            this.StoreService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected IStoreService<T> StoreService { get; }

        [HttpPost]
        public virtual async Task<Result<T>> Add([FromBody]T item)
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

                var result = await this.StoreService.Add(item);
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

        [HttpGet]
        public virtual Task<T> GetById(string id) => this.StoreService.GetBy(c => c.Id, id);

        [HttpDelete]
        public virtual Task<Result> Delete(string id) => this.StoreService.Delete(id);

        [HttpPatch]
        public virtual async Task<Result<T>> Patch(string id, string propertyName, object value)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Cannot be empty", nameof(id));
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

                var result = await this.StoreService.Patch(id, exp, changedValue);
                return result.ToCamelCasedPropertiesResult();
            }
        }

        protected virtual async Task<int> Count()
        {
            var count = await this.StoreService.Count();
            return count;
        }
    }
}
