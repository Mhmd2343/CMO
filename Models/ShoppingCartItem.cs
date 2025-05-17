using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryAppSystem.Models
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public string ShoppingCartId { get; set; } // Could be session ID or user ID
   
    }
}
