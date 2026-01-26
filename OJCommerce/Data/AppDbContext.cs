using Microsoft.EntityFrameworkCore;
using OJCommerce.Models.Carts;
using OJCommerce.Models.Categories;
using OJCommerce.Models.Coupons;
using OJCommerce.Models.Orders;
using OJCommerce.Models.PaymentMethods;
using OJCommerce.Models.Products;
using OJCommerce.Models.Roles;
using OJCommerce.Models.Shipments;
using OJCommerce.Models.Tokens;
using OJCommerce.Models.Transactions;
using OJCommerce.Models.Users;
using OJCommerce.Models.Vendors;
using OJCommerce.Models.Webhooks;

namespace OJCommerce.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<PaymentWebhookEvent> PaymentWebhookEvents { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<SavedPaymentMethod> SavedPaymentMethods { get; set; }
        public DbSet<Shipment> Shipments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PAYMENT TRANSACTIONS
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.Property(p => p.Provider)
                      .HasConversion<string>()
                      .HasMaxLength(50);

                entity.Property(p => p.Method)
                      .HasConversion<string>()
                      .HasMaxLength(50);

                entity.Property(p => p.Status)
                      .HasConversion<string>()
                      .HasMaxLength(50);
            });

            // PAYMENT WEBHOOK EVENTS
            modelBuilder.Entity<PaymentWebhookEvent>(entity =>
            {
                entity.Property(w => w.Provider)
                      .HasConversion<string>()
                      .HasMaxLength(50);

                entity.Property(w => w.Status)
                      .HasConversion<string>()
                      .HasMaxLength(50);

                entity.Property(w => w.CardReusable)
                      .HasConversion<int>(); // bool → 0/1

                entity.Property(w => w.Processed)
                      .HasConversion<int>(); // bool → 0/1
            });

            //SHIPMENT STATUS
            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasIndex(s => s.OrderId).IsUnique();

                entity.HasOne(s => s.Order)
                      .WithOne()
                      .HasForeignKey<Shipment>(s => s.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

    }

}
