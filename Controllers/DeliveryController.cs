using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DeliveryAppSystem.Data;
using DeliveryAppSystem.Models;

namespace DeliveryAppSystem.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeliveryController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Display form to create a new delivery request (Client side)
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
                return RedirectToAction("Login", "User");
            return View();
        }

        /// <summary>
        /// Handle POST of new delivery request (Client side)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DeliveryRequest model)
        {
            var clientIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(clientIdStr))
                return RedirectToAction("Login", "User");

            model.ClientId = int.Parse(clientIdStr);
            model.Status = RequestStatus.Pending;
            model.CreatedAt = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                _context.DeliveryRequests.Add(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        /// <summary>
        /// List all delivery requests for the logged-in client
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            var clientIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(clientIdStr))
                return RedirectToAction("Login", "User");

            int clientId = int.Parse(clientIdStr);
            var requests = _context.DeliveryRequests
                .Where(r => r.ClientId == clientId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(requests);
        }

        /// <summary>
        /// Show details for a specific delivery request
        /// </summary>
        [HttpGet]
        public IActionResult Details(int id)
        {
            var request = _context.DeliveryRequests
                .FirstOrDefault(r => r.RequestId == id);
            if (request == null)
                return NotFound();
            return View(request);
        }

        /// <summary>
        /// Rate a driver after delivery (Driver rating flow)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateDriver(int driverId, int score, string comment)
        {
            var customerIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(customerIdStr))
            {
                ViewBag.Error = "You need to be logged in to rate a driver.";
                return RedirectToAction("Login", "User");
            }

            var rating = new Rating
            {
                DriverID = driverId,
                CustomerID = int.Parse(customerIdStr),
                Score = score,
                Comment = comment
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            var driver = await _context.Drivers
                .Include(d => d.Ratings)
                .FirstOrDefaultAsync(d => d.UserId == driverId);
            if (driver != null)
            {
                // Recalculate average rating (cast double to decimal)
                driver.AverageRating = (decimal)driver.Ratings.Average(r => r.Score);
                _context.Drivers.Update(driver);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("DriverDetails", "Driver", new { id = driverId });
        }
    }
}
