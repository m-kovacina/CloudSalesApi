using CloudSales.Api.Implementation.Domain;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Api.Data
{
    public class CloudSalesDbContext : DbContext
    {
        public CloudSalesDbContext(DbContextOptions<CloudSalesDbContext> options) : base(options)
        {
        }

        public DbSet<Customer?> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<PurchasedSoftware> PurchasedSoftware { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.CustomerIdentifier)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
