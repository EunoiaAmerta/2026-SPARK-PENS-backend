using System.ComponentModel.DataAnnotations;

namespace SparkPens.Api.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = "User"; // "Admin" or "User"
    
    [MaxLength(255)]
    public string? GoogleId { get; set; }
    
    // For admin login - not stored as plain text in production!
    [MaxLength(255)]
    public string? PasswordHash { get; set; }
    
    // Flag to check if user has set a password (for Google login users)
    public bool HasPassword { get; set; } = false;
    
    // Password reset fields
    [MaxLength(255)]
    public string? ResetToken { get; set; }
    
    public DateTime? ResetExpiry { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

