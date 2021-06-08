using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates;

namespace Ordering.Infrastructure.Configs
{
    class OrderStatusConfiguration
        : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> cardTypesConfiguration)
        {

        }
    }
}
