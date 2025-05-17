using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryAppSystem.Models
{
    public enum OrderStatus
    {
        Pending,
        Preparing,
        Ready,
        Delivered,
        Cancelled,
        Assigned  // ✅ Add this line if it's missing
    }


    public class ShopOrder
    {
        public int ShopOrderId { get; set; } // Primary Key

        public int ClientId { get; set; }
        public Client Client { get; set; }  // Navigation property

        public int ShopId { get; set; }
        public Shop Shop { get; set; }  // Navigation property

        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public int? AssignedDriverId { get; set; }
        public Driver AssignedDriver { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }

        public string RequestChange { get; set; }

        public decimal TotalAmountUSD { get; set; }

        public decimal TotalAmountLBP { get; set; }

        public string? PromoCode { get; set; }

        public string EstimatedDeliveryTime { get; set; }

        public List<ShopOrderItem> Items { get; set; } = new List<ShopOrderItem>();
    }
}
