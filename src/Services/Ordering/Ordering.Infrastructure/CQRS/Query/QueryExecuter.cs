using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.CQRS
{
    public interface IQueryExecuter
    {
        Task<TResult> Execute<TResult>(IQuery<TResult> query);
    }
    public class QueryExecuter : IQueryExecuter
    {
        private readonly IServiceProvider serviceProvider;

        public QueryExecuter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task<TResult> Execute<TResult>(IQuery<TResult> query)
        {
            using var scope = serviceProvider.CreateScope();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            var handler = scope.ServiceProvider.GetService(handlerType);
            if (handler == default)
                throw new Exception($"There is no query handler in DI container configured for {query.GetType()}");

            var handleMethod = handlerType
                    .GetMethod("Handle");

            return await (Task<TResult>)handleMethod.Invoke(handler, new object[] { query });
        }
    }
}
