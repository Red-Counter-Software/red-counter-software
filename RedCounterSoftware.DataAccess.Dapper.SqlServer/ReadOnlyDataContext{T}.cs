namespace RedCounterSoftware.DataAccess.Dapper.SqlServer
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Dapper;
    using RedCounterSoftware.Common;
    using RedCounterSoftware.Common.Extensions;

    public abstract class ReadOnlyDataContext<T> : IReadDataContext<T>
        where T : RecordBase
    {
        private readonly string tableName;

        private readonly string schemaName;

        protected SqlConnection SqlConnection { get; private set; }

        private bool disposedValue; // To detect redundant calls

        public ReadOnlyDataContext(string connectionString, string tableName, string schemaName = "dbo")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.tableName = !string.IsNullOrEmpty(tableName) ? tableName : throw new ArgumentNullException(nameof(tableName));
            this.schemaName = schemaName;
            this.SqlConnection = new SqlConnection(connectionString);
        }

        public virtual Task<int> Count(CancellationToken cancellationToken = default)
        {
            var command = $"Select Count(*) From [{this.schemaName}].[{this.tableName}]";
            return this.SqlConnection.ExecuteScalarAsync<int>(command);
        }

        public virtual Task<bool> ExistsBy<TK>(System.Linq.Expressions.Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            var fieldName = selector.GetPropertyName();
            var command = $"Select Count(Distinct 1) From [{this.schemaName}].[{this.tableName}] Where [{fieldName}] = @Value";
            return this.SqlConnection.ExecuteScalarAsync<bool>(command, new { Value = value });
        }

        public virtual Task<T> GetBy<TK>(System.Linq.Expressions.Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default)
        {
            var fieldName = selector.GetPropertyName();
            var command = $"Select * From [{this.schemaName}].[{this.tableName}] Where [{fieldName}] = @Value";
            return this.SqlConnection.QuerySingleOrDefaultAsync<T>(command, new { Value = value });
        }

        public virtual async Task<SearchResult<T>> GetByMultipleValues<TK>(System.Linq.Expressions.Expression<Func<T, TK>> selector, TK[] values, CancellationToken cancellationToken = default)
        {
            var fieldName = selector.GetPropertyName();
            var command = $"Select * From [{this.schemaName}].[{this.tableName}] Where [{fieldName}] IN @Values";
            var result = await this.SqlConnection.QueryAsync<T>(command, new { Values = values }).ConfigureAwait(false);
            var items = result.ToArray();
            return new SearchResult<T>(items.Count(), items);
        }

        public virtual async Task<SearchResult<T>> Search(SearchParameters<T> searchParameters, CancellationToken cancellationToken = default)
        {
            var command = this.ComposeSearch(searchParameters);
            command = await this.SearchFilters(command, searchParameters);
            var result = await this.SqlConnection.QueryAsync<T>(command.ToString(), new { Value = searchParameters.SearchTerm }).ConfigureAwait(false);
            var items = result.ToArray();
            return new SearchResult<T>(items.Count(), items);
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
                    this.SqlConnection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.disposedValue = true;
            }
        }

        protected abstract StringBuilder ComposeSearch(SearchParameters<T> searchParameters);

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

            command = command
                .AppendLine($"Order By {searchParameters.SortTerm} {sortDirection}")
                .AppendLine($"Offset {offset} Rows")
                .AppendLine($"Fetch Next {searchParameters.PageSize} Rows Only");

            return Task.FromResult(command);
        }
    }
}
