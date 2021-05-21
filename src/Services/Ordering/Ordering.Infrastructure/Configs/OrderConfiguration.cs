using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Configs
{
    class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(b => b.Id)
                 .UseHiLo("orderseq", OrderingContext.DEFAULT_SCHEMA);

            builder
                .OwnsOne(o => o.ShippingAddress, a =>
                {
                    // Explicit configuration of the shadow key property in the owned type 
                    // as a workaround for a documented issue in EF Core 5: https://github.com/dotnet/efcore/issues/20740
                    a.Property<int>("OrderId")
                    .UseHiLo("orderseq", OrderingContext.DEFAULT_SCHEMA);
                    a.WithOwner();
                });
            builder
                .OwnsOne(o => o.InvoiceAddress, a =>
                {
                    // Explicit configuration of the shadow key property in the owned type 
                    // as a workaround for a documented issue in EF Core 5: https://github.com/dotnet/efcore/issues/20740
                    a.Property<int>("OrderId")
                    .UseHiLo("orderseq", OrderingContext.DEFAULT_SCHEMA);
                    a.WithOwner();
                });
        }
    }
}
