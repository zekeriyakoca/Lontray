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
    class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.Property(b => b.Id)
                 .UseHiLo("buyerseq", OrderingContext.DEFAULT_SCHEMA);
        }
    }
}
