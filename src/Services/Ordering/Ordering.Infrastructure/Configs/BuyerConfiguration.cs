using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates;

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
