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
    class OrderStatusConfiguration
        : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> cardTypesConfiguration)
        {

        }
    }
}
