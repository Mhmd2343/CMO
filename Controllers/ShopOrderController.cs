using DeliveryAppSystem.Data;
using Microsoft.AspNetCore.Mvc;
using DeliveryAppSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAppSystem.Controllers
{
    public class ShopOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            ViewData["Products"] = _context.Products.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(List<int> productIds, List<int> quantities)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "User");
            }
            var order = new ShopOrder
            {
                ClientId = userId,
                OrderDate = DateTime.Now,
                Items = new List<ShopOrderItem>()
            };


            for (int i = 0; i < productIds.Count; i++)
            {
                order.Items.Add(new ShopOrderItem
                {
                    ProductId = productIds[i],
                    Quantity = quantities[i]
                });
            }

            _context.ShopOrders.Add(order);
            _context.SaveChanges();
            Console.WriteLine("Order saved for client ID: " + userId);

            return RedirectToAction("Index", "Shop");
        }

        public IActionResult UserOrders()
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                ViewBag.Message = "User not logged in.";
                return RedirectToAction("Login", "User");
            }

            // Fetch orders from DB
            var orders = _context.ShopOrders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.ClientId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.OrderCount = orders.Count; // 💡 Add this to see the count in the view

            return View(orders);
        }
        [HttpPost]
        public IActionResult AssignDriverManually(int orderId, int driverId)
        {
            var order = _context.ShopOrders.FirstOrDefault(o => o.ShopOrderId == orderId);
            var driver = _context.Drivers.FirstOrDefault(d => d.DriverId == driverId);

            if (order == null || driver == null)
            {
                TempData["Error"] = "Invalid order or driver!";
                return RedirectToAction("Details", "Driver", new { id = driverId });
            }

            if (order.AssignedDriverId != null)
            {
                TempData["Error"] = "This order already has a driver assigned!";
                return RedirectToAction("Details", "Driver", new { id = driverId });
            }

            order.AssignedDriverId = driverId;
            order.Status = DeliveryAppSystem.Models.OrderStatus.Assigned;
            _context.SaveChanges();

            TempData["Success"] = $"Driver successfully assigned to order #{orderId}!";
            return RedirectToAction("Details", "Driver", new { id = driverId });
        }

    }

}
