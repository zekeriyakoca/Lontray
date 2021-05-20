using Autofac;
using EventBus.Events.Interfaces;
using Ordering.Infrastructure.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

            //TODO : Test following generic injection
            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(t => t.GetNestedTypes().Contains(typeof(QueryHandler<,>)))
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
