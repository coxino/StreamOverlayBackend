using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamApi.DatabaseContext
{
    internal class AccountsConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("accounts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.username).HasMaxLength(10).IsRequired(true);
            builder.Property(x => x.password).HasMaxLength(50).IsRequired(true);
            builder.Property(x => x.isbanned).IsRequired(true).HasDefaultValue(0);
            builder.Property(x => x.created).IsRequired(true);
            builder.Property(x => x.subscription).IsRequired(true);
            builder.Property(x => x.role).HasMaxLength(1).IsRequired(true).HasDefaultValue(0);
        }
    }
}