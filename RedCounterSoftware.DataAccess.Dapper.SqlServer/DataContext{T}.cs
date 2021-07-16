namespace RedCounterSoftware.DataAccess.Dapper.SqlServer
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Dapper;
    using RedCounterSoftware.Common;
    using RedCounterSoftware.Common.Extensions;

    public abstract class DataContext<T> : ReadOnlyDataContext<T>, IDataContext<T>
        where T : RecordBase
    {
        public DataContext(string connectionString, string tableName, string schemaName = "dbo")
            : base(connectionString, tableName, schemaName)
        {
        }

        public abstract Task<T> Add<TId>(Expression<Func<T, TId>> filter, TId id, T toAdd, CancellationToken cancellationToken = default);

        public abstract Task<T[]> AddBulk<TId>(Expression<Func<T, TId>> filter, T[] toAdd, CancellationToken cancellationToken = default);

        public async Task Delete<TId>(Expression<Func<T, TId>> filter, TId id, CancellationToken cancellationToken = default)
        {
            var identifierName = filter.GetPropertyName();
            var command = $"Delete From [{this.SchemaName}].[{this.TableName}] Where [{identifierName}] = @Id";
            using var connection = await this.GetSqlConnection(cancellationToken);
            _ = await connection.ExecuteAsync(command, new { Id = id });
        }

        public async Task<T> Patch<TId, TK>(Expression<Func<T, TId>> filter, TId id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            var identifierName = filter.GetPropertyName();
            var fieldName = selector.GetPropertyName();
            var command = $"Update [{this.SchemaName}].[{this.TableName}] Set [{fieldName}] = @Value Where [{identifierName}] = @Id";
            using var connection = await this.GetSqlConnection(cancellationToken);
            _ = await connection.ExecuteAsync(command, new { Value = value, Id = id }).ConfigureAwait(false);
            var result = await this.GetBy(filter, id).ConfigureAwait(false);
            return result;
        }
    }
}
