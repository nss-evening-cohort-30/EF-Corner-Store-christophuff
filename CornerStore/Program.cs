using CornerStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core and provides dummy value for testing
builder.Services.AddNpgsql<CornerStoreDbContext>(builder.Configuration["CornerStoreDbConnectionString"] ?? "testing");

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Cashier Endpoints
app.MapGet("/cashiers", (CornerStoreDbContext db) =>
{
    return db.Cashiers
        .Include(c => c.Orders)
            .ThenInclude(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
        .ToList();
});

app.MapPost("/cashiers", (CornerStoreDbContext db, Cashier cashier) =>
{
    db.Cashiers.Add(cashier);
    db.SaveChanges();
    return Results.Created($"/cashiers/{cashier.Id}", cashier);
});

app.MapGet("/cashiers/{id}", (CornerStoreDbContext db, int id) =>
{
    var cashier = db.Cashiers
        .Include(c => c.Orders)
            .ThenInclude(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
        .FirstOrDefault(c => c.Id == id);
    
    return cashier == null ? Results.NotFound() : Results.Ok(cashier);
});

// Product Endpoints
app.MapGet("/products", (CornerStoreDbContext db, string? search) =>
{
    var products = db.Products.Include(p => p.Category);

    if (!string.IsNullOrEmpty(search))
    {
        return products.Where(p => p.ProductName.ToLower().Contains(search.ToLower()) ||
                                  p.Category.CategoryName.ToLower().Contains(search.ToLower()))
                      .ToList();
    }

    return products.ToList();
});

app.MapPost("/products", (CornerStoreDbContext db, Product product) =>
{
    db.Products.Add(product);
    db.SaveChanges();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id}", (CornerStoreDbContext db, int id, Product product) =>
{
    var existingProduct = db.Products.FirstOrDefault(p => p.Id == id);
    if (existingProduct == null) return Results.NotFound();

    existingProduct.ProductName = product.ProductName;
    existingProduct.Price = product.Price;
    existingProduct.Brand = product.Brand;
    existingProduct.CategoryId = product.CategoryId;

    db.SaveChanges();
    return Results.NoContent();
});

// Order Endpoints
app.MapGet("/orders/{id}", (CornerStoreDbContext db, int id) =>
{
    var order = db.Orders
        .Include(o => o.Cashier)
        .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Category)
        .FirstOrDefault(o => o.Id == id);
    
    return order == null ? Results.NotFound() : Results.Ok(order);
});

app.MapGet("/orders", (CornerStoreDbContext db, string? orderDate) =>
{
    var orders = db.Orders
        .Include(o => o.Cashier)
        .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product) 
                .ThenInclude(p => p.Category);    
    if (!string.IsNullOrEmpty(orderDate))
    {
        if (DateTime.TryParse(orderDate, out DateTime date))
        {
            return orders.Where(o => o.PaidOnDate.HasValue && o.PaidOnDate.Value.Date == date.Date).ToList();
        }
    }    
    return orders.ToList();
});

app.MapDelete("/orders/{id}", (CornerStoreDbContext db, int id) =>
{
    var order = db.Orders.FirstOrDefault(o => o.Id == id);
    if (order == null) return Results.NotFound();

    db.Orders.Remove(order);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapPost("/orders", (CornerStoreDbContext db, Order order) =>
{
    db.Orders.Add(order);
    db.SaveChanges();

    // Re-load the order with related data
    var createdOrder = db.Orders
        .Include(o => o.Cashier)
        .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Category)
        .FirstOrDefault(o => o.Id == order.Id);
    
    return Results.Created($"/orders/{order.Id}", createdOrder);
});

app.MapPost("/orders/{orderId}/products", (CornerStoreDbContext db, int orderId, OrderProduct orderProduct) =>
{
    orderProduct.OrderId = orderId;
    db.OrderProducts.Add(orderProduct);
    db.SaveChanges();
    
    // Re-load with product data
    var createdOrderProduct = db.OrderProducts
        .Include(op => op.Product)
        .Include(op => op.Order)
        .FirstOrDefault(op => op.OrderId == orderId && op.ProductId == orderProduct.ProductId);
    
    return Results.Created($"/orders/{orderId}/products", createdOrderProduct);
});

app.Run();

//don't move or change this!
public partial class Program { }