using Inventory.Helper;
using Inventory.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }  
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<IncomingItem> IncomingItems { get; set; }
        public DbSet<ItemOut> ItemsOut { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                 new Role { Id = 1, RoleName = "superadmin" },
                 new Role { Id = 2, RoleName = "admin" }
             );

            modelBuilder.Entity<User>().HasData(
                 new User { Id = 1, Name = "Dwi Purnomo", Username = "dwi", Password = PasswordHasher.HashPassword("password"), RoleId = 1 },
                 new User { Id = 2, Name = "Robert Davis Chaniago", Username = "robert", Password = PasswordHasher.HashPassword("password"), RoleId = 2 }
            ); 
        }
    }
}
