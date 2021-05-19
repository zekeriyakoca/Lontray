using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Common
{
    public static class DomainEvents
    {
        private static ServiceProvider provider;

        public static void Init(IServiceCollection buildInContainer)
        {
            provider = buildInContainer.BuildServiceProvider();
        }

        public static async Task Raise<T>(T args) where T : IDomainEvent
        {
            if (provider != null)
            {
                foreach (var handler in provider.GetServices<IHandler<T>>())
                {
                    await handler.Handle(args);
                }
            }
        }

        public static async Task Call<TEvent, TParam>(TParam param) where TEvent : IEventHandler<TParam>
        {
            foreach (var handler in provider.GetServices<TEvent>())
            {
                await handler.Handle(param);
            }
        }


    }
}
