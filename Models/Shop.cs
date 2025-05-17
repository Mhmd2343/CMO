using DeliveryAppSystem.Models;
namespace DeliveryAppSystem.Models
{
    public class Shop
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public ICollection<Product> Products { get; set; }
    }

}
