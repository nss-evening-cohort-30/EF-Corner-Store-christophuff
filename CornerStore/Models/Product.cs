using System.ComponentModel.DataAnnotations;

namespace CornerStore.Models;

public class Product
{
    public int Id { get; set; }
        
    [Required]
    public string ProductName { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public string Brand { get; set; }
    
    [Required]
    public int CategoryId { get; set; }  // Foreign key
    
    // Navigation property - this product belongs to one category
    public Category Category { get; set; }
    
    // Navigation property - this product can be in many order products
    public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}