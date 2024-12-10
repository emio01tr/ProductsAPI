using Microsoft.EntityFrameworkCore;

namespace ProductsAPI.Models
{
    public class ProductsContext(DbContextOptions<ProductsContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(
            new Product {id=1,ProductName="Iphone 14",Price=60000,IsActive=true},
            new Product {id=2,ProductName="Iphone 13",Price=50000,IsActive=true},
            new Product {id=3,ProductName="Iphone 12",Price=40000,IsActive=true},
            new Product {id=4,ProductName="Iphone 11",Price=30000,IsActive=false}
            );
        }
        public DbSet<Product> Products { get; set; } = null!;
    }
}