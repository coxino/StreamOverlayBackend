using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseContext
{
    internal class WinnerConfiguration : IEntityTypeConfiguration<Winner>
    {
        public void Configure(EntityTypeBuilder<Winner> builder)
        {
            builder.ToTable("Winners");
            builder.HasKey(x => x.Id);
        }
    }
}