using Microsoft.AspNetCore.Mvc;
using DeliveryAppSystem.Data;
using DeliveryAppSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq; // Important for FirstOrDefault!

namespace DeliveryAppSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: User/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View("SelectRegisterType"); // Show selection view first
        }

        // GET: User/RegisterDriver
        [HttpGet]
        public IActionResult RegisterDriver()
        {
            return View(); // Loads Views/User/RegisterDriver.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterDriver(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string ConfirmPassword,
        string VehicleType,
        string PlateNumber,
        string PricingType,
        string WorkingHours,
        decimal? PricePerKm,
        decimal? FixedDeliveryPrice)
        {
            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View();
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (existingUser != null)
            {
                ViewBag.Error = "User with this email already exists!";
                return View();
            }

            // 1. Create User entity
            var user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Role = "Driver"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            // 2. Create Driver entity linked to the new user
            var driver = new Driver
            {
                UserId = user.UserId,
                VehicleType = VehicleType,
                PlateNumber = PlateNumber,
                PricingType = PricingType,
                WorkingHours = WorkingHours,
                PricePerKm = PricePerKm ?? 0,
                FixedDeliveryPrice = FixedDeliveryPrice ?? 0,
                AverageRating = 0.0m,
                Ratings = new List<Rating>()
            };

            _context.Add(driver);
            _context.SaveChanges();

            return RedirectToAction("Login");
        
        }

        // GET: User/RegisterCustomer
        [HttpGet]
        public IActionResult RegisterCustomer()
        {
            return View(); // Loads Views/User/RegisterCustomer.cshtml
        }

        // POST: User/RegisterCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterCustomer(
            string FirstName,
            string LastName,
            string Email,
            string Password,
            string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View();
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (existingUser != null)
            {
                ViewBag.Error = "User with this email already exists!";
                return View();
            }

            var user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Role = "Customer"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // GET: User/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter email and password.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("UserID", user.UserId.ToString());
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // Redirect to original page if available
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid login attempt.";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        // GET: User/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
