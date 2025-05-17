using System.Collections.Generic;

namespace DeliveryAppSystem.Models.ViewModels
{
    public class DriverDashboardViewModel
    {
        public Driver Driver { get; set; }
        public List<ShopOrder> AssignedOrders { get; set; }
    }
}
