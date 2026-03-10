using Ecommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Ecommerce.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<CustomerRank> CustomerRanks { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<CartDetails> Cart_Menus { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Revenue> Revenues { get; set; }
        public DbSet<Revenue_Order> Revenue_Orders { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Discount_Customer> Discount_Customers { get; set; }
        public DbSet<FoodImage> FoodImages { get; set; }
        public DbSet<FoodSize> FoodSizes { get; set; }
        public DbSet<OTP> OTPs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            //== Customer ==

            //Customer - CustomerRank
            builder.Entity<Customer>()
            .HasOne(kh => kh.CustomerRank)
            .WithMany()
            .HasForeignKey(kh => kh.RankId);

            //== Cart ==

            //Cart - Table
            builder.Entity<Cart>()
                .HasOne(o => o.Table)
                .WithMany()
                .HasForeignKey(o => o.TableId);

            //Cart - Customer
            builder.Entity<Cart>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId);

            //== Menu ==

            //Menu - MenuCategory
            builder.Entity<Menu>()
                .HasOne(o => o.MenuCategory)
                .WithMany()
                .HasForeignKey(o => o.MenuCategoryId);

            //== Cart_Menu ==
            builder.Entity<CartDetails>()
               .HasKey(ct => new { ct.CartId, ct.FoodSizeId });

            builder.Entity<CartDetails>()
                .HasOne(ct => ct.FoodSize)
                .WithMany(hd => hd.CartDetails)
                .HasForeignKey(ct => ct.FoodSizeId);

            builder.Entity<CartDetails>()
                .HasOne(ct => ct.Cart)
                .WithMany(v => v.CartDetails)
                .HasForeignKey(ct => ct.CartId);

            //== Menu_Order ==
            builder.Entity<OrderDetails>()
               .HasKey(ct => new { ct.FoodSizeId, ct.OrderId});

            builder.Entity<OrderDetails>()
                .HasOne(ct => ct.FoodSize)
                .WithMany(hd => hd.OrderDetails)
                .HasForeignKey(ct => ct.FoodSizeId);

            builder.Entity<OrderDetails>()
                .HasOne(ct => ct.Order)
                .WithMany(v => v.OrderDetails)
                .HasForeignKey(ct => ct.OrderId);

            //== Inventory ==
            builder.Entity<Inventory>()
                .HasOne(o => o.FoodSize)
                .WithMany()
                .HasForeignKey(o => o.FoodSizeId);

            //== Revenue_Order ==
            builder.Entity<Revenue_Order>()
               .HasKey(ct => new { ct.RevenueId, ct.OrderId });

            builder.Entity<Revenue_Order>()
                .HasOne(ct => ct.Revenue)
                .WithMany(hd => hd.Revenue_Order)
                .HasForeignKey(ct => ct.RevenueId);

            builder.Entity<Revenue_Order>()
                .HasOne(ct => ct.Order)
                .WithMany(v => v.Revenue_Order)
                .HasForeignKey(ct => ct.OrderId);

            //== Discount_Customer ==
            builder.Entity<Discount_Customer>()
               .HasKey(ct => new { ct.CustomerId, ct.DiscountId });

            builder.Entity<Discount_Customer>()
                .HasOne(ct => ct.Customer)
                .WithMany(hd => hd.Discount_Customer)
                .HasForeignKey(ct => ct.CustomerId);

            builder.Entity<Discount_Customer>()
                .HasOne(ct => ct.Discount)
                .WithMany(v => v.Discount_Customer)
                .HasForeignKey(ct => ct.DiscountId);

           
            //== FoodImage ==
            builder.Entity<FoodImage>()
                .HasOne(f => f.Menu)
                .WithMany(m => m.FoodImages)
                .HasForeignKey(f => f.MenuId)
                .OnDelete(DeleteBehavior.Cascade);


            //== FoodSize ==
            builder.Entity<FoodSize>()
                .HasOne(o => o.Menu)
                .WithMany()
                .HasForeignKey(o => o.MenuId);

            //== OTP ==
            builder.Entity<OTP>()
                .HasOne(o => o.Users)
                .WithMany()
                .HasForeignKey(o => o.UserId);

            base.OnModelCreating(builder);
        }
    }
}
