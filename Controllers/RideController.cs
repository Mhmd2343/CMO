using Microsoft.AspNetCore.Mvc;
using DeliveryAppSystem.Data;
using DeliveryAppSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryAppSystem.Controllers
{
    public class RideController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RideController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ride/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ride/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RideRequest ride)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "User");

            ride.CustomerId = int.Parse(userId);
            ride.Status = "Pending";
            ride.RequestTime = DateTime.Now;

            _context.RideRequests.Add(ride);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyRides");
        }

        // GET: Ride/MyRides
        public async Task<IActionResult> MyRides()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "User");

            var rides = await _context.RideRequests
                .Include(r => r.Driver)
                .Where(r => r.CustomerId.ToString() == userId)
                .ToListAsync();

            return View(rides);
        }

        // GET: Ride/Pending (for drivers to view unassigned ride requests)
        public async Task<IActionResult> Pending()
        {
            var rides = await _context.RideRequests
                .Include(r => r.Customer)
                .Where(r => r.Status == "Pending")
                .ToListAsync();

            return View(rides);
        }

        // POST: Ride/Accept
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var driverId = HttpContext.Session.GetString("DriverID");
            if (string.IsNullOrEmpty(driverId))
                return RedirectToAction("Login", "Driver");

            var ride = await _context.RideRequests.FindAsync(id);
            if (ride == null || ride.Status != "Pending")
                return NotFound();

            ride.DriverId = int.Parse(driverId);
            ride.Status = "Accepted";

            _context.Update(ride);
            await _context.SaveChangesAsync();

            return RedirectToAction("AssignedRides");
        }

        // POST: Ride/Decline
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(int id)
        {
            var ride = await _context.RideRequests.FindAsync(id);
            if (ride == null || ride.Status != "Pending")
                return NotFound();

            ride.Status = "Declined";

            _context.Update(ride);
            await _context.SaveChangesAsync();

            return RedirectToAction("Pending");
        }

        // GET: Ride/AssignedRides (for drivers to view their accepted rides)
        public async Task<IActionResult> AssignedRides()
        {
            var driverId = HttpContext.Session.GetString("DriverID");
            if (string.IsNullOrEmpty(driverId))
                return RedirectToAction("Login", "Driver");

            var rides = await _context.RideRequests
                .Include(r => r.Customer)
                .Where(r => r.DriverId.ToString() == driverId)
                .ToListAsync();

            return View(rides);
        }

        // POST: Ride/Complete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var driverId = HttpContext.Session.GetString("DriverID");
            if (string.IsNullOrEmpty(driverId))
                return RedirectToAction("Login", "Driver");

            var ride = await _context.RideRequests.FindAsync(id);
            if (ride == null || ride.DriverId.ToString() != driverId || ride.Status != "Accepted")
                return NotFound();

            ride.Status = "Completed";

            _context.Update(ride);
            await _context.SaveChangesAsync();

            return RedirectToAction("AssignedRides");
        }

        // POST: Ride/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var ride = await _context.RideRequests.FindAsync(id);
            if (ride == null || ride.Status == "Completed" || ride.Status == "Cancelled")
                return NotFound();

            ride.Status = "Cancelled";

            _context.Update(ride);
            await _context.SaveChangesAsync();

            var driverId = HttpContext.Session.GetString("DriverID");
            return string.IsNullOrEmpty(driverId) ? RedirectToAction("MyRides") : RedirectToAction("AssignedRides");
        }

        // GET: Ride/Rate/5
        public async Task<IActionResult> Rate(int id)
        {
            var ride = await _context.RideRequests
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ride == null || ride.Status != "Completed")
                return NotFound();

            return View(ride);
        }

        // POST: Ride/Rate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(int id, int rating, string review)
        {
            var ride = await _context.RideRequests.FindAsync(id);
            if (ride == null || ride.Status != "Completed")
                return NotFound();

            ride.Rating = rating;
            ride.Review = review;

            _context.Update(ride);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyRides");
        }
    }
}
