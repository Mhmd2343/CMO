using DeliveryAppSystem.Models;

public class ShopOrderItem
{
    public int ShopOrderItemId { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }

    public int ShopOrderId { get; set; }
    public ShopOrder ShopOrder { get; set; }
}
