using System.ComponentModel.DataAnnotations;

namespace CornerStore.Models;

public class Order
{
    public int Id { get; set; }
        
    [Required]
    public int CashierId { get; set; }  // Foreign key
    
    // Calculated property from OrderProducts
    public decimal Total 
    { 
        get 
        { 
            return OrderProducts?.Sum(op => op.Product?.Price * op.Quantity) ?? 0; 
        } 
    }
    
    public DateTime? PaidOnDate { get; set; }
    
    // Navigation properties
    public Cashier Cashier { get; set; }
    // One order can have many order products
    public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}