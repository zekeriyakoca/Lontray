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

        public QueryHandler(OrderingSettings options)
        {
            connectionString = options?.ConnectionString ?? throw new ArgumentNullException(nameof(options.ConnectionString));
        }

        public async Task<TResult> Handle(TQuery query)
        {
            try
            {
                connection = new SqlConnection(connectionString);

                var result = await Action(query);

                return result;

            }
            catch
            {
                //TODO : Implement
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
