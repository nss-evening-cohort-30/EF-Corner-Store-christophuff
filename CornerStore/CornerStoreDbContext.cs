using Microsoft.EntityFrameworkCore;
using CornerStore.Models;
public class CornerStoreDbContext : DbContext
{

    public CornerStoreDbContext(DbContextOptions<CornerStoreDbContext> context) : base(context)
    {

    }
    
    public DbSet<Cashier> Cashiers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }

    //allows us to configure the schema when migrating as well as seed data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // OrderProduct has no id, this creates a primary key
        modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

        // One Order to many OrderProducts relationship 
        modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

        // One Product to many OrderProducts relationship
        modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId);

        
        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, CategoryName = "Snacks" },
            new Category { Id = 2, CategoryName = "Beverages" },
            new Category { Id = 3, CategoryName = "Candy" }
        );

        // Seed Cashiers
        modelBuilder.Entity<Cashier>().HasData(
            new Cashier { Id = 1, FirstName = "Chris", LastName = "Huff" },
            new Cashier { Id = 2, FirstName = "Tony", LastName = "Hawk" }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, ProductName = "Chips", Price = 2.50m, Brand = "Lays", CategoryId = 1 },
            new Product { Id = 2, ProductName = "Soda", Price = 1.99m, Brand = "Coke", CategoryId = 2 },
            new Product { Id = 3, ProductName = "Chocolate", Price = 1.25m, Brand = "Hershey", CategoryId = 3 }
        );

        // Seed Orders
        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, CashierId = 1, PaidOnDate = DateTime.Now.AddDays(-1) },
            new Order { Id = 2, CashierId = 2, PaidOnDate = null }
        );

        // Seed OrderProducts
        modelBuilder.Entity<OrderProduct>().HasData(
            new OrderProduct { OrderId = 1, ProductId = 1, Quantity = 2 },
            new OrderProduct { OrderId = 1, ProductId = 2, Quantity = 1 },
            new OrderProduct { OrderId = 2, ProductId = 3, Quantity = 3 }
        );
    }
}