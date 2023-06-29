using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderTaker.Models;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security.AccessControl;

namespace OrderTaker.Data
{
    public class OrderTakerDbContext : IdentityDbContext<User>
    {
        public OrderTakerDbContext(DbContextOptions<OrderTakerDbContext> options)
            : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<SKU> SKUs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<Customer>().ToTable(nameof(Customer))
                .HasMany(c => c.PurchaseOrders)
                .WithOne(c => c.Customer)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<PurchaseOrder>().ToTable(nameof(PurchaseOrder))
                .HasMany(po => po.PurchaseItems)
                .WithOne(c => c.PurchaseOrder)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<PurchaseItem>().ToTable(nameof(PurchaseItem));
            builder.Entity<SKU>().ToTable(nameof(SKU));

            builder.Entity<Customer>()
                .Property(c => c.FullName);

            builder.Entity<Customer>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("getdate()");
            builder.Entity<Customer>()
                .Property(b => b.TimeStamp)
                .HasDefaultValueSql("getdate()");

            builder.Entity<SKU>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("getdate()");
            builder.Entity<SKU>()
               .Property(b => b.TimeStamp)
               .HasDefaultValueSql("getdate()");

            builder.Entity<PurchaseOrder>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("getdate()");
            builder.Entity<PurchaseOrder>()
               .Property(b => b.TimeStamp)
               .HasDefaultValueSql("getdate()");

            builder.Entity<PurchaseItem>()
               .Property(b => b.TimeStamp)
               .HasDefaultValueSql("getdate()");

        }
    }
}