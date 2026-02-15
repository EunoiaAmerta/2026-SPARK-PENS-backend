using System.ComponentModel.DataAnnotations;

namespace SparkPens.Api.DTOs
{
    public class UpdateStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;

        public string? RejectionReason { get; set; }
    }
}

