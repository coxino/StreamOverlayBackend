using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseContext
{
    internal class GivewayTiketConfiguration : IEntityTypeConfiguration<GivewayTiket>
    {
        public void Configure(EntityTypeBuilder<GivewayTiket> builder)
        {
            builder.ToTable("GiveawayTikets");
            builder.HasKey(x => x.Id);
        }
    }
}