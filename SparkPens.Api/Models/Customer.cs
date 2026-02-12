using System.ComponentModel.DataAnnotations;

namespace SparkPens.Api.Models;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "active"; // active/inactive
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Fitur Soft Delete sesuai kriteria
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}