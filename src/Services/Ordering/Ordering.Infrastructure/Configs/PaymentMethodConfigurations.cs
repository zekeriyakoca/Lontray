using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates;

namespace Ordering.Infrastructure.Configs
{
    public class PaymentMethodConfigurations
        : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.Property(b => b.Id)
                .UseHiLo("paymentseq", OrderingContext.DEFAULT_SCHEMA);
        }
    }
}
