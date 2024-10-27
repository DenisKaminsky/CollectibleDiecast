using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollectibleDiecast.Ordering.API.Data.EntityConfigurations;

class ClientRequestEntityTypeConfiguration : IEntityTypeConfiguration<ClientRequest>
{
    public void Configure(EntityTypeBuilder<ClientRequest> requestConfiguration)
    {
        requestConfiguration.ToTable("requests");
    }
}
