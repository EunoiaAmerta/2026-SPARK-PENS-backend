using System.ComponentModel.DataAnnotations;

namespace SparkPens.Api.DTOs;

public class CustomerDto
{
    [Required(ErrorMessage = "Nama wajib diisi")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public string? Address { get; set; }
    
    [RegularExpression("active|inactive", ErrorMessage = "Status harus active atau inactive")]
    public string Status { get; set; } = "active";
}