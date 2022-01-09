using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.CQRS
{
    public interface IQueryHandler<TQuery, TResult>
    {
        public Task<TResult> Handle(TQuery query);
    }

    public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>, IDisposable
    {
        protected SqlConnection connection;
        private readonly string connectionString;
        public ILogger<QueryHandler<TQuery, TResult>> logger { get; }

        public QueryHandler(OrderingSettings options, ILogger<QueryHandler<TQuery, TResult>> logger)
        {
            // Connection pooling is provided through ConnectionString
            connectionString = options?.ConnectionString ?? throw new ArgumentNullException(nameof(options.ConnectionString));
            this.logger = logger;
        }

        public async Task<TResult> Handle(TQuery query)
        {
            try
            {
                connection = new SqlConnection(connectionString);

                var result = await Action(query);

                return result;

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }


        public abstract Task<TResult> Action(TQuery query);

        public void Dispose()
        {
            if (connection != null)
            {
                if (connection.State == System.Data.ConnectionState.Open) connection.Close();
                connection.Dispose();
            }
        }

    }
}
