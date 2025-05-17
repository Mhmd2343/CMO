using DeliveryAppSystem.Models;
namespace DeliveryAppSystem.Models;
public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }

    public int ShopId { get; set; }
    public Shop Shop { get; set; }

    // ✅ Add this for the category
    public int ProductCategoryId { get; set; }
    public ProductCategory Category { get; set; }
}
