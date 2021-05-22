using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ordering.Domain.Common
{
    public class DomainEvents
    {
        private readonly IServiceProvider provider;

        public DomainEvents(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public async Task Raise<T>(T args) where T : IDomainEvent
        {
            using var scope = provider.CreateScope();
            foreach (var handler in scope.ServiceProvider.GetServices<IHandler<T>>())
            {
                await handler.Handle(args);
            }
        }

    }
}
