using Microsoft.EntityFrameworkCore;
using OJCommerce.Models.Carts;
using OJCommerce.Models.Categories;
using OJCommerce.Models.Coupons;
using OJCommerce.Models.Orders;
using OJCommerce.Models.Products;
using OJCommerce.Models.Roles;
using OJCommerce.Models.Tokens;
using OJCommerce.Models.Transactions;
using OJCommerce.Models.Users;
using OJCommerce.Models.Vendors;

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

    }


}
