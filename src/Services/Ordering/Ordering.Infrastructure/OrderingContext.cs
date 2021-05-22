using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Common;
using Ordering.Infrastructure.Configs;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Infrastructure
{
    public class OrderingContext : DbContext
    {
        private readonly DbContextOptions options;
        private readonly IHttpContextAccessor accessor;

        public OrderingContext([NotNullAttribute] DbContextOptions options, IHttpContextAccessor accessor) : base(options)
        {
            this.options = options;
            this.accessor = accessor;
        }

        public const string DEFAULT_SCHEMA = "ordering";

        public DbSet<Order> Orders { get; set; }
        public DbSet<Buyer> Buyers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(PaymentMethodConfigurations)));
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await OnSaveChangesAsync();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            OnSaveChangesAsync().Wait();
            return base.SaveChanges();
        }

        private async Task OnSaveChangesAsync()
        {
            var domainEntities = ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());


            var domainEvents = domainEntities
                 .SelectMany(x => x.Entity.DomainEvents)
                 .ToList();

            await HandleDomainEvents(domainEntities, domainEvents);
        }

        private async Task HandleDomainEvents(IEnumerable<EntityEntry<Entity>> domainEntities, List<IDomainEvent> domainEvents)
        {
            if (domainEvents.Count > 0)
            {
                domainEntities.ToList()
                    .ForEach(entity => entity.Entity.ClearDomainEvents());

                foreach (var e in domainEvents)
                {
                    var handlerType = typeof(IHandler<>).MakeGenericType(e.GetType());

                    var handler = accessor.HttpContext.RequestServices.GetService(handlerType);

                    var handleMethod = handlerType
                        .GetMethod("Handle");

                    await (Task)handleMethod.Invoke(handler, new object[] { e });
                };
            }
        }
    }
}
