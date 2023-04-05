using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseContext
{
    internal class AccountsConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("accounts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Username).HasMaxLength(10).IsRequired(true);
            builder.Property(x => x.Password).HasMaxLength(50).IsRequired(true);
            builder.Property(x => x.IsBanned).IsRequired(true).HasDefaultValue(0);
            builder.Property(x => x.Created).IsRequired(true);
            builder.Property(x => x.Subscription).IsRequired(true);
            builder.Property(x => x.YoutubeToken).IsRequired(false);
            builder.Property(x => x.Role).HasMaxLength(1).IsRequired(true).HasDefaultValue(0);
        }
    }
}