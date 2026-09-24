using InventoryInvoiceApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryInvoiceApp.Data
{
    public class AppDbContext
        : IdentityDbContext<AppUser>
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Provider> Providers { get; set; }

        public DbSet<Material> Materials { get; set; }

        public DbSet<ProviderMaterial> ProviderMaterials
        {
            get;
            set;
        }

        public DbSet<CashierRecord> CashierRecords
        {
            get;
            set;
        }

        public DbSet<StockMovement> StockMovements
        {
            get;
            set;
        }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<Material>()
                .HasIndex(x => x.MaterialName)
                .IsUnique();

            builder.Entity<ProviderMaterial>()
                .HasOne(x => x.Provider)
                .WithMany(x => x.ProviderMaterials)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProviderMaterial>()
                .HasOne(x => x.Material)
                .WithMany(x => x.ProviderMaterials)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CashierRecord>()
                .HasOne(x => x.Material)
                .WithMany(x => x.CashierRecords)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}