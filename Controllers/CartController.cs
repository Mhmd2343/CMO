using DeliveryAppSystem.Data;
using DeliveryAppSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DeliveryAppSystem.Services
{
    public class CartController : Controller
    {
        private readonly ShoppingCart _cart;
        private readonly ApplicationDbContext _context;

        public CartController(ShoppingCart cart, ApplicationDbContext context)
        {
            _cart = cart;
            _context = context;
        }

        public IActionResult Index()
        {
            var items = _cart.GetCartItems();
            return View(items);
        }

        public IActionResult AddToCart(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                _cart.AddToCart(product, 1);
            }

            return RedirectToAction("Products", "Shop", new { shopId = product.ShopId });
        }

        public IActionResult Clear()
        {
            _cart.ClearCart();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, string change)
        {
            if (change == "increase")
            {
                _cart.AddToCart(productId); // ✅ Adds 1 more to cart
            }
            else if (change == "decrease")
            {
                _cart.DecreaseQuantity(productId); // ✅ Decreases or removes item
            }

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int productId)
        {
            // Optional: You can implement full removal (regardless of quantity)
            var item = _cart.GetCartItems().FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    _cart.DecreaseQuantity(productId);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
