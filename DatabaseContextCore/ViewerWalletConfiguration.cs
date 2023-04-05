using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContextCore
{
    internal class ViewerWalletConfiguration : IEntityTypeConfiguration<ViewerWallet>
    {
        public void Configure(EntityTypeBuilder<ViewerWallet> builder)
        {
            builder.ToTable("ViewerWallets");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.StreamerId).HasMaxLength(50).IsRequired(true);
            builder.Property(x => x.ViewerId).HasMaxLength(100).IsRequired(true);
            builder.Property(x => x.Coins).IsRequired(true);
        }
    }
}
