using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Aggregates;
using Ordering.Domain.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Infrastructure
{
    public class OrderingContext : DbContext
    {
        private readonly DbContextOptions options;

        public OrderingContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
            this.options = options;
        }

        public const string DEFAULT_SCHEMA = "ordering";

        public DbSet<Order> Orders { get; set; }
        public DbSet<Buyer> Buyers { get; set; }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnSaveChanges();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            OnSaveChanges();
            return base.SaveChanges();
        }

        private void OnSaveChanges()
        {
            var domainEntities = ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());


            var domainEvents = domainEntities
                 .SelectMany(x => x.Entity.DomainEvents)
                 .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            var raiseMethodInfo = typeof(DomainEvents)
                   .GetMethod("Raise");

            domainEvents.ForEach(e =>
            {
                raiseMethodInfo.MakeGenericMethod(e.GetType())
                   .Invoke(null, new object[] { e });
            });
        }
    }
}
