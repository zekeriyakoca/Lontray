using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ordering.Domain.Common
{
    // Most likely we won't need this class since we are adding Domaing events to Entity's property
    // and raise events during EF SaveChanges()
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
