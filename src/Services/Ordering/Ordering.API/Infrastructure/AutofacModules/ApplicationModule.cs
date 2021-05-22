using Autofac;
using EventBus.Events.Interfaces;
using Ordering.Infrastructure.CQRS;
using System;
using System.Linq;
using System.Reflection;

namespace Ordering.API.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        public ApplicationModule()
        {

        }

        protected override void Load(ContainerBuilder builder)
        {
            var currentAssembly = Assembly.GetAssembly(typeof(Startup));

            //Add QueryHandlers to Container
            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(t => t.FullName.EndsWith("QueryHandler"))
                .PreserveExistingDefaults()
                .AsImplementedInterfaces();

            //Add IntegrationHandlers to Container
            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(t => t.GetInterfaces().Contains(typeof(IIntegrationEventHandler)))
                .PreserveExistingDefaults()
                .AsImplementedInterfaces();
        }
    }
}
