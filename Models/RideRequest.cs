using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryAppSystem.Models
{
    public class RideRequest
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [Required]
        public string PickupLocation { get; set; }

        [Required]
        public string DropoffLocation { get; set; }

        public DateTime RequestTime { get; set; } = DateTime.Now;

        public int? DriverId { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Accepted, Completed, Cancelled

        [ForeignKey("CustomerId")]
        public User Customer { get; set; }

        [ForeignKey("DriverId")]
        public Driver? Driver { get; set; }
        public int? Rating { get; set; } // 1 to 5
        public string? Review { get; set; }

    }
}
