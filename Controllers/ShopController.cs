using DeliveryAppSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeliveryAppSystem.Models;
using DeliveryAppSystem.Services;

namespace DeliveryAppSystem.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCart _cart; // ✅ Injected cart

        // ✅ Modify this constructor to accept the cart
        public ShopController(ApplicationDbContext context, ShoppingCart cart)
        {
            _context = context;
            _cart = cart;
        }

        public IActionResult Index()
        {
            var shops = _context.Shops.Include(s => s.Products).ToList();
            return View(shops);
        }

        public IActionResult Products(int shopId, int? categoryId)
        {
            var shop = _context.Shops
                .Include(s => s.Products)
                .ThenInclude(p => p.Category)
                .FirstOrDefault(s => s.ShopId == shopId);

            if (shop == null) return NotFound();

            var products = shop.Products.AsQueryable();

            if (categoryId.HasValue)
                products = products.Where(p => p.ProductCategoryId == categoryId.Value);

            var cartItems = _cart.GetCartItems(); // ✅ Now it works!
            ViewBag.CartQuantities = cartItems.ToDictionary(i => i.Product.ProductId, i => i.Quantity);

            ViewBag.ShopName = shop.Name;
            ViewBag.Categories = _context.ProductCategories.ToList();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.ShopId = shopId;

            return View(products.ToList());
        }
    }
}
