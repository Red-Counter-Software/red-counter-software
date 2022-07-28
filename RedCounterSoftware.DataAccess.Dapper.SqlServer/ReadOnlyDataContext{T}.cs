namespace RedCounterSoftware.DataAccess.Dapper.SqlServer
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Dapper;
    using RedCounterSoftware.Common;
    using RedCounterSoftware.Common.Extensions;

    public abstract class ReadOnlyDataContext<T> : IReadDataContext<T>
        where T : RecordBase
    {
        private bool disposedValue; // To detect redundant calls

        protected ReadOnlyDataContext(string connectionString, string tableName, string schemaName = "dbo")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.ConnectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
            this.TableName = !string.IsNullOrWhiteSpace(tableName) ? tableName : throw new ArgumentNullException(nameof(tableName));
            this.SchemaName = schemaName;
        }

        protected string ConnectionString { get; private set; }

        protected string TableName { get; private set; }

        protected string SchemaName { get; private set; }

        public virtual async Task<int> Count(CancellationToken cancellationToken = default)
        {
            var command = $"Select Count(*) From [{this.SchemaName}].[{this.TableName}]";
            using var connection = await this.GetSqlConnection(cancellationToken).ConfigureAwait(false);
            return await connection.ExecuteScalarAsync<int>(command).ConfigureAwait(false);
        }

        public virtual async Task<bool> ExistsBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            var fieldName = selector.GetPropertyName();
            var command = $"Select Count(Distinct 1) From [{this.SchemaName}].[{this.TableName}] Where [{fieldName}] = @Value";
            using var connection = await this.GetSqlConnection(cancellationToken).ConfigureAwait(false);
            return await connection.ExecuteScalarAsync<bool>(command, new { Value = value }).ConfigureAwait(false);
        }

        public virtual async Task<T?> GetBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            var fieldName = selector.GetPropertyName();
            var command = $"Select * From [{this.SchemaName}].[{this.TableName}] Where [{fieldName}] = @Value";
            using var connection = await this.GetSqlConnection(cancellationToken).ConfigureAwait(false);
            return await connection.QuerySingleOrDefaultAsync<T>(command, new { Value = value }).ConfigureAwait(false);
        }

        public virtual async Task<SearchResult<T>> GetByMultipleValues<TK>(Expression<Func<T, TK>> selector, TK[] values, CancellationToken cancellationToken = default)
        {
            var fieldName = selector.GetPropertyName();
            var command = $"Select * From [{this.SchemaName}].[{this.TableName}] Where [{fieldName}] IN @Values";
            using var connection = await this.GetSqlConnection(cancellationToken).ConfigureAwait(false);
            var result = await connection.QueryAsync<T>(command, new { Values = values }).ConfigureAwait(false);
            var items = result.ToArray();
            return new SearchResult<T>(items.Length, items);
        }

        public virtual async Task<SearchResult<T>> Search(SearchParameters<T> searchParameters, CancellationToken cancellationToken = default)
        {
            if (searchParameters is null)
            {
                throw new ArgumentNullException(nameof(searchParameters));
            }

            var command = this.ComposeSearch(searchParameters);
            command = await this.SearchFilters(command, searchParameters).ConfigureAwait(false);
            using var connection = await this.GetSqlConnection(cancellationToken).ConfigureAwait(false);
            var result = await connection.QueryAsync<T>(command.ToString(), new { Value = searchParameters.SearchTerm }).ConfigureAwait(false);
            var items = result.ToArray();
            return new SearchResult<T>(items.Length, items);
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
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.disposedValue = true;
            }
        }

        protected async Task<SqlConnection> GetSqlConnection(CancellationToken cancellationToken)
        {
            var sqlConnection = new SqlConnection(this.ConnectionString);
            await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return sqlConnection;
        }

        protected virtual StringBuilder ComposeSearch(SearchParameters<T> searchParameters)
        {
            var builder = new StringBuilder();
#pragma warning disable CA1305 // Specify IFormatProvider
            return builder.AppendLine($"Select * From [{this.SchemaName}].[{this.TableName}]");
#pragma warning restore CA1305 // Specify IFormatProvider
        }

        protected virtual Task<StringBuilder> SearchFilters(StringBuilder command, SearchParameters<T> searchParameters)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (searchParameters == null)
            {
                throw new ArgumentNullException(nameof(searchParameters));
            }

            var offset = searchParameters.CurrentPage * searchParameters.PageSize;
            var sortDirection = searchParameters.IsDescending ? "Desc" : "Asc";

#pragma warning disable CA1305 // Specify IFormatProvider
            command = command
                .AppendLine($"Order By {searchParameters.SortTerm} {sortDirection}")
                .AppendLine($"Offset {offset} Rows")
                .AppendLine($"Fetch Next {searchParameters.PageSize} Rows Only");
#pragma warning restore CA1305 // Specify IFormatProvider

            return Task.FromResult(command);
        }
    }
}
