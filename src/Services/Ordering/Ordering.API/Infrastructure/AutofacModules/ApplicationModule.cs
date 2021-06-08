using Autofac;
using EventBus.Events.Interfaces;
using Ordering.API;
using System;
using System.Linq;
using System.Reflection;

namespace Ordering.Infrastructure.AutofacModules
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

            //Add CommandHandlers to Container
            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(t => t.FullName.EndsWith("CommandHandler"))
                .PreserveExistingDefaults()
                .AsImplementedInterfaces();

            //Add IntegrationHandlers to Container
            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(t => t.GetInterfaces().Contains(typeof(IIntegrationEventHandler)))
                .PreserveExistingDefaults()
                .AsImplementedInterfaces();

            //Add DomainEventhandlers to Container
            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(t => t.FullName.EndsWith("DomainEventHandler"))
                .PreserveExistingDefaults()
                .AsImplementedInterfaces();
        }

    }
}
