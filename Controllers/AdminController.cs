using Microsoft.AspNetCore.Mvc;
using DeliveryAppSystem.Data;
using DeliveryAppSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace DeliveryAppSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Admin> _passwordHasher;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Admin>();
        }


        // GET: Admin
        public IActionResult Index()
        {
            var admins = _context.Admins.ToList();
            return View(admins);
        }

        // GET: Admin/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Admin admin)
        {
            if (ModelState.IsValid)
            {
                // Hash and store password
                admin.PasswordHash = _passwordHasher.HashPassword(admin, admin.Password);
                admin.Password = null; // clear plaintext

                _context.Admins.Add(admin);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admin/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var admin = _context.Admins.Find(id);
            if (admin == null) return NotFound();
            return View(admin);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Admin admin)
        {
            if (id != admin.AdminId) return NotFound();
            if (ModelState.IsValid)
            {
                // If password field is filled, rehash
                if (!string.IsNullOrEmpty(admin.Password))
                {
                    admin.PasswordHash = _passwordHasher.HashPassword(admin, admin.Password);
                }
                admin.Password = null;

                _context.Admins.Update(admin);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admin/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var admin = _context.Admins.Find(id);
            if (admin == null) return NotFound();
            return View(admin);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var admin = _context.Admins.Find(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email);
            if (admin != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("AdminId", admin.AdminId.ToString());
                    return RedirectToAction("Dashboard");
                }
            }
            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        // GET: Admin/Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId))
                return RedirectToAction("Login");

            ViewBag.AdminId = adminId;
            return View();
        }

        // GET: Admin/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction("Login");
        }
        // Add at the bottom of AdminController

        [HttpGet]
        public IActionResult AssignDriver()
        {
            var orders = _context.ShopOrders
                .Include(o => o.Client)
                .Include(o => o.AssignedDriver) // FIXED here
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Preparing) // use existing enum values
                .ToList();

            var drivers = _context.Drivers.Include(d => d.User).ToList();

            ViewBag.Drivers = drivers;
            return View(orders);
        }

        [HttpPost]
        public IActionResult AssignDriverToOrder(int orderId, int driverId)
        {
            var order = _context.ShopOrders.Find(orderId);
            if (order != null)
            {
                order.AssignedDriverId = driverId;
                order.Status = OrderStatus.Preparing; // use existing enum
                _context.SaveChanges();
            }

            return RedirectToAction("AssignDriver");
        }
        // GET: Admin/ManageMenu
        public IActionResult ManageMenu()
        {
            var products = _context.Products.Include(p => p.Category).Include(p => p.Shop).ToList();
            return View(products);
        }

        // GET: Admin/CreateProduct
        [HttpGet]
        public IActionResult CreateProduct()
        {
            ViewBag.Categories = _context.ProductCategories.ToList();
            // Removed ViewBag.Shops because only one shop exists
            return View();
        }

        // POST: Admin/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                product.ShopId = 1; // Force to CMO Market
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(ManageMenu));
            }

            ViewBag.Categories = _context.ProductCategories.ToList();
            // No ViewBag.Shops here either
            return View(product);
        }

        // GET: Admin/EditProduct/5
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            ViewBag.Categories = _context.ProductCategories.ToList();
            // Removed ViewBag.Shops here as well
            return View(product);
        }

        // POST: Admin/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();
            if (ModelState.IsValid)
            {
                product.ShopId = 1; // Keep it tied to CMO Market
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(ManageMenu));
            }

            ViewBag.Categories = _context.ProductCategories.ToList();
            // No ViewBag.Shops here either
            return View(product);
        }

        // GET: Admin/DeleteProduct/5
        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Admin/DeleteProduct/5
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProductConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(ManageMenu));
        }
        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = _context.ShopOrders.Find(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                _context.SaveChanges();
            }
            return RedirectToAction("AssignDriver");
        }


    }
}

    

