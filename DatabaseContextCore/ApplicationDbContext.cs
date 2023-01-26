using DatabaseContextCore;
using Microsoft.EntityFrameworkCore;

namespace DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Viewer> Viewers { get; set; }
        public DbSet<GivewayModel> Giveways { get; set; }
        public DbSet<GivewayTiket> GivewayTikets { get; set; }
        public DbSet<Winner> Winners { get; set; }
        public DbSet<ViewerWallet> ViewerWallets { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new AccountsConfiguration());
            modelBuilder.ApplyConfiguration(new ViewerConfiguration());
            modelBuilder.ApplyConfiguration(new GivewayModelConfiguration());
            modelBuilder.ApplyConfiguration(new GivewayTiketConfiguration());
            modelBuilder.ApplyConfiguration(new WinnerConfiguration());
            modelBuilder.ApplyConfiguration(new ViewerWalletConfiguration());
        }
    }
}
