using System.ComponentModel.DataAnnotations;

namespace CornerStore.Models;

public class Cashier
{
    public int Id { get; set; }
        
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    // Computed property - not stored in database
    public string FullName => $"{FirstName} {LastName}";
    
    // Navigation property - one cashier can handle many orders
    public List<Order> Orders { get; set; } = new List<Order>();
}