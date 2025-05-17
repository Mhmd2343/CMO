using Microsoft.AspNetCore.Mvc;
using DeliveryAppSystem.Data;
using DeliveryAppSystem.Models;
using DeliveryAppSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text;

namespace DeliveryAppSystem.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        // ✅ NEW: Order History Page for Logged-In Client
        public IActionResult OrderHistory()
        {
            if (!HttpContext.Session.TryGetValue("UserId", out byte[] userIdBytes))
            {
                return RedirectToAction("Login");
            }

            int userId = int.Parse(Encoding.UTF8.GetString(userIdBytes));

            var client = _context.Clients
                .FirstOrDefault(c => c.UserId == userId);

            if (client == null)
            {
                return RedirectToAction("Login");
            }
            var orders = _context.ShopOrders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.ClientId == client.UserId)  // ✅ Use UserId instead
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var viewModel = new ClientOrderHistoryViewModel
            {
                ClientName = $"{client.FirstName} {client.LastName}",
                Orders = orders
            };

            return View(viewModel);
        }
        // ClientsController.cs

        public IActionResult BrowseDrivers(string city = null, decimal? minRating = null)
        {
            var drivers = _context.Drivers
                .Include(d => d.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                drivers = drivers.Where(d => d.City == city);
            }

            if (minRating.HasValue)
            {
                drivers = drivers.Where(d => d.AverageRating >= minRating.Value);
            }

            var driverList = drivers.ToList();
            return View(driverList);
        }


    }
}
