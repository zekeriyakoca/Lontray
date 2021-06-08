using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates;

namespace Ordering.Infrastructure.Configs
{
    class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(b => b.Id)
                 .UseHiLo("orderitemseq", OrderingContext.DEFAULT_SCHEMA);
        }
    }
}
