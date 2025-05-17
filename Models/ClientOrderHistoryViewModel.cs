using System.Collections.Generic;
using DeliveryAppSystem.Models;

namespace DeliveryAppSystem.Models.ViewModels
{
    public class ClientOrderHistoryViewModel
    {
        public string ClientName { get; set; }
        public List<ShopOrder> Orders { get; set; }
    }
}
