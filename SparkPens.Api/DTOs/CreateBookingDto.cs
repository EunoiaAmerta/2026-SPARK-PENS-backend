using System.ComponentModel.DataAnnotations;

namespace SparkPens.Api.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public string RequesterName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string RequesterEmail { get; set; } = string.Empty;

        [Required]
        public string RequesterPhone { get; set; } = string.Empty;

        [Required]
        public DateTime BookingStartDate { get; set; }

        [Required]
        public DateTime BookingEndDate { get; set; }

        [Required]
        public string Purpose { get; set; } = string.Empty;
    }
}
