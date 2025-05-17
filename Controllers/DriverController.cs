using DeliveryAppSystem.Data;
using DeliveryAppSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DeliveryAppSystem.Models.ViewModels;

namespace DeliveryAppSystem.Controllers
{
    public class DriverController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public DriverController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: Driver/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Driver/Login
        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                ViewBag.Error = "Invalid email!";
                return View();
            }

            // Find corresponding Driver
            var driver = _context.Drivers.FirstOrDefault(d => d.UserId == user.UserId);
            if (driver == null)
            {
                ViewBag.Error = "This user is not a registered driver!";
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Password);
            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid password!";
                return View();
            }

            // Save session info
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserRole", "Driver");

            return RedirectToAction("Dashboard", "Driver");
        }

        // GET: Driver/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Driver/Register
        [HttpPost]
        public IActionResult Register(
            string FirstName,
            string LastName,
            string Email,
            string Password,
            string ConfirmPassword,
            string VehicleType,
            string PlateNumber,
            string PricingType,
            decimal Price,
            string City,
            string WorkingHours)
        {
            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View();
            }

            bool emailExists = _context.Users.Any(u => u.Email == Email)
                            || _context.Clients.Any(c => c.Email == Email);

            if (emailExists)
            {
                ViewBag.Error = "A user with this email already exists!";
                return View();
            }

            // Step 1: Create User
            var user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, Password);

            _context.Users.Add(user);
            _context.SaveChanges(); // Save to get the generated UserId

            // Step 2: Create Driver and link to user
            var driver = new Driver
            {
                UserId = user.UserId,
                VehicleType = VehicleType,
                PlateNumber = PlateNumber,
                PricingType = PricingType,
                City = City,
                WorkingHours = WorkingHours,
                PricePerKm = PricingType == "PerKM" ? Price : 0,
                FixedDeliveryPrice = PricingType == "PerDelivery" ? Price : 0,
                AverageRating = 0
            };

            _context.Drivers.Add(driver);
            _context.SaveChanges();

            return RedirectToAction("Login", "Driver");
        }

        public IActionResult Dashboard()
        {
            // Get logged-in user ID from session
            if (!HttpContext.Session.TryGetValue("UserId", out byte[] userIdBytes))
            {
                return RedirectToAction("Login");
            }

            int userId = int.Parse(System.Text.Encoding.UTF8.GetString(userIdBytes));

            // Find the driver
            var driver = _context.Drivers
                .Include(d => d.User)
                .FirstOrDefault(d => d.UserId == userId);

            if (driver == null)
            {
                return RedirectToAction("Login");
            }

            // Get assigned orders (ShopOrder or DeliveryRequest depending on your current setup)
            var assignedShopOrders = _context.ShopOrders
                .Include(o => o.Client)
                .Include(o => o.Items)
                .Where(o => o.AssignedDriverId == driver.DriverId)
                .ToList();

            var model = new DriverDashboardViewModel
            {
                Driver = driver,
                AssignedOrders = assignedShopOrders
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            var order = _context.ShopOrders.FirstOrDefault(o => o.ShopOrderId == orderId);
            if (order != null)
            {
                if (Enum.TryParse<OrderStatus>(status, out var parsedStatus))
                {
                    order.Status = parsedStatus;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Dashboard");
        }

        // GET: Driver/Browse
        [HttpGet]
        public IActionResult Browse(string city, decimal? minRating)
        {
            var drivers = _context.Drivers
                .Include(d => d.User)
                .Where(d =>
                    (string.IsNullOrEmpty(city) || d.City == city) &&
                    (!minRating.HasValue || d.AverageRating >= minRating.Value))
                .ToList();

            ViewBag.Cities = _context.Drivers
                .Select(d => d.City)
                .Distinct()
                .ToList();

            return View(drivers);
        }
        // GET: Driver/Details/{id}
        [HttpGet]
        public IActionResult Details(int id)
        {
            var driver = _context.Drivers
                .Include(d => d.User)
                .FirstOrDefault(d => d.DriverId == id);

            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

    }
}
