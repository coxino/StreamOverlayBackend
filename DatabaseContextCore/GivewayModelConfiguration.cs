using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseContext
{
    internal class GivewayModelConfiguration : IEntityTypeConfiguration<GivewayModel>
    {
        public void Configure(EntityTypeBuilder<GivewayModel> builder)
        {
            builder.ToTable("Giveaways");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(x => x.Description).HasMaxLength(255).IsRequired(true);
            builder.Property(x => x.WinnersCount).IsRequired(true).HasDefaultValue(0);
            builder.Property(x => x.EndTime).IsRequired(true);
            builder.Property(x => x.MaxTikets).IsRequired(true).HasDefaultValue(0);
        }
    }
}