using System.ComponentModel.DataAnnotations;

namespace CornerStore.Models;

public class OrderProduct
{
    [Required]
    public int ProductId { get; set; }  // Foreign key to Product
    
    [Required]
    public int OrderId { get; set; }    // Foreign key to Order
    
    [Required]
    public int Quantity { get; set; } 
    
    // Navigation properties
    public Product Product { get; set; }
    public Order Order { get; set; }
}