using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Aggregates;

namespace Ordering.Infrastructure.Configs
{
    class CardTypeConfiguration
        : IEntityTypeConfiguration<CardType>
    {
        public void Configure(EntityTypeBuilder<CardType> cardTypesConfiguration)
        {
            //cardTypesConfiguration.ToTable("CardTypes", OrderingContext.DEFAULT_SCHEMA);

            //cardTypesConfiguration.HasKey(ct => ct.Id);

            //cardTypesConfiguration.Property(ct => ct.Id)
            //    .HasDefaultValue(1)
            //    .ValueGeneratedNever()
            //    .IsRequired();

            //cardTypesConfiguration.Property(ct => ct.Name)
            //    .HasMaxLength(200)
            //    .IsRequired();
        }
    }
}
