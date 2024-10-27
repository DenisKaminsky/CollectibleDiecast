using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CardType = CollectibleDiecast.Ordering.API.Data.Models.CardType;

namespace CollectibleDiecast.Ordering.API.Data.EntityConfigurations;

class CardTypeEntityTypeConfiguration : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> cardTypesConfiguration)
    {
        cardTypesConfiguration.ToTable("cardtypes");

        cardTypesConfiguration.Property(ct => ct.Id)
            .ValueGeneratedNever();

        cardTypesConfiguration.Property(ct => ct.Name)
            .HasMaxLength(200);
    }
}
