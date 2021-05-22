using Ordering.Infrastructure.CQRS;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.CQRS
{
    public interface ICommandHandler<TCommand, TResult>
    {
        public Task<TResult> Execute(TCommand query);
    }

    public abstract class CommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    {
        public readonly OrderingContext context;

        public CommandHandler(OrderingContext context)
        {
            this.context = context;
        }

        public async Task<TResult> Execute(TCommand command)
        {
            var result = await Action(command);

            return result;
        }

        public abstract Task<TResult> Action(TCommand query);



    }
}
