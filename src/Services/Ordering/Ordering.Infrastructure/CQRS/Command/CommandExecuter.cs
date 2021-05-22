using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.CQRS
{
    public interface ICommandExecuter
    {
        Task<TResult> Execute<TResult>(ICommand<TResult> query);
    }
    public class CommandExecuter : ICommandExecuter
    {
        private readonly IServiceProvider serviceProvider;

        public CommandExecuter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task<TResult> Execute<TResult>(ICommand<TResult> query)
        {
            using var scope = serviceProvider.CreateScope();
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            var handler = scope.ServiceProvider.GetService(handlerType);
            if (handler == default)
                throw new Exception($"There is no query handler in DI container configured for {query.GetType()}");

            var handleMethod = handlerType
                    .GetMethod("Execute");

            return await (Task<TResult>)handleMethod.Invoke(handler, new object[] { query });
        }
    }
}
