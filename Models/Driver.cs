using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryAppSystem.Models
{
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }

        // Link to User (Foreign Key)
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        // Driver-specific fields
        public string VehicleType { get; set; }
        public string PlateNumber { get; set; }
        public string PricingType { get; set; }  // "PerKM" or "PerDelivery"
        public string City { get; set; }
        public string WorkingHours { get; set; }

        public decimal FixedDeliveryPrice { get; set; }
        public decimal PricePerKm { get; set; }

        public decimal AverageRating { get; set; } = 0.0m;

        public List<Rating> Ratings { get; set; }

        // Price calculator method
        public decimal CalculatePrice(decimal distance)
        {
            return PricingType == "PerKM" ? distance * PricePerKm :
                   PricingType == "PerDelivery" ? FixedDeliveryPrice : 0;
        }
    }
}
