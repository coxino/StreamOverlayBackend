using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DatabaseContext
{
    internal class TwitchViewerConfiguration : IEntityTypeConfiguration<Viewer>
    {
        public void Configure(EntityTypeBuilder<Viewer> builder)
        {
            builder.ToTable("TwitchViewers");
            builder.HasKey(x=>x.Id);
            builder.Property(x => x.Id).HasMaxLength(50).IsRequired(true);
            builder.Property(x => x.Email).HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.EmailSecundar).HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.CreationTime).IsRequired(true);
            builder.Property(x => x.LastActive).IsRequired(true);
            builder.Property(x => x.Ipadress).HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.Name).HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(false);
            builder.Property(x => x.ExpiresMember).IsRequired(true);
            
            builder.HasMany(x=>x.Wallets).WithOne(x=>x.Viewer).HasForeignKey(x=>x.ViewerId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}