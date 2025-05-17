using System;
using System.Collections.Generic;
using DeliveryAppSystem.Models;

namespace DeliveryAppSystem.Models
{
    public class OrderItemDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderConfirmationViewModel
    {
        public int ShopOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string RequestChange { get; set; }
        public decimal TotalAmountUSD { get; set; }
        public decimal TotalAmountLBP { get; set; }
        public string PromoCode { get; set; }
        public string EstimatedDeliveryTime { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Discount { get; set; } // ✅ Add this
        public List<OrderItemDto> Items { get; set; }
        public Driver? AssignedDriver { get; set; }

    }

}
