using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkPens.Api.Models
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid RoomId { get; set; }
        
        [ForeignKey("RoomId")]
        public Room? Room { get; set; }

        [Required]
        [StringLength(100)]
        public string RequesterName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string RequesterEmail { get; set; } = string.Empty;

        public string? RequesterPhone { get; set; }

        // Legacy field for backwards compatibility
        [Obsolete("Use BookingStartDate and BookingEndDate instead")]
        public DateTime? BookingDate { get; set; }

        [Required]
        public DateTime BookingStartDate { get; set; }

        [Required]
        public DateTime BookingEndDate { get; set; }

        [Required]
        public string Purpose { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}