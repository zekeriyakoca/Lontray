using Catalog.API.Entities;
using DomainHelper.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure
{
    public class CatalogContext : DbContext
    {
        private readonly DbContextOptions options;

        public CatalogContext(DbContextOptions options) : base(options)
        {
            this.options = options;
        }

        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            string sql = "getdate()";

            if (options.Extensions != null)
            {
                foreach (var ext in options.Extensions)
                {
                    if (ext.GetType().ToString().StartsWith("Microsoft.EntityFrameworkCore.Sqlite"))
                    {
                        sql = "DATE('now')";
                        break;
                    }
                }
            }


            modelBuilder.Entity<CatalogItem>().Property(p => p.LastCreationTime).HasDefaultValueSql(sql);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);

        }

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
            var entriesModified = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IModificationAudited
                            && e.State == EntityState.Modified)
                .Select(e => e.Entity as IModificationAudited);

            foreach (var entityEntry in entriesModified)
            {
                entityEntry.LastModificationTime = DateTime.Now;
            }

            //var entriesChanged = ChangeTracker
            //  .Entries()
            //  .Where(e => e.Entity is ICreationAudited
            //              && e.State == EntityState.Added)
            //  .Select(e => e.Entity as ICreationAudited);

            //foreach (var entityEntry in entriesChanged)
            //{
            //    entityEntry.LastCreationTime = DateTime.Now;

            //}
        }


    }
}
