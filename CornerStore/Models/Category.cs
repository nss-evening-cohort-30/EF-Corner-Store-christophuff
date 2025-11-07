using System.ComponentModel.DataAnnotations;

namespace CornerStore.Models;

public class Category
{
    public int Id { get; set; }
        
    [Required]
    public string CategoryName { get; set; }
    
    // Navigation property - one category can have many products
    public List<Product> Products { get; set; } = new List<Product>();
}