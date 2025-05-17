using System.Collections.Generic;

namespace DeliveryAppSystem.Models
{
    public class CheckoutViewModel
    {
        public string DeliveryAddress { get; set; }
        public string RequestChangeOption { get; set; } // e.g., "None", "50", "100"
        public string PromoCode { get; set; }

        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalUSD { get; set; }
        public decimal TotalLBP => TotalUSD * 90000;
        public List<Driver> AvailableDrivers { get; set; } = new();
        public int? SelectedDriverId { get; set; }

        public string EstimatedDeliveryTime => "20 - 40 minutes";

        public List<ShoppingCartItem> CartItems { get; set; }
    }
}
