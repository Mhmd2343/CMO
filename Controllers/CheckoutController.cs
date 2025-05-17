using DeliveryAppSystem.Data;
using DeliveryAppSystem.Models;
using DeliveryAppSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DeliveryAppSystem.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCart _cart;

        public CheckoutController(ApplicationDbContext context, ShoppingCart cart)
        {
            _context = context;
            _cart = cart;
        }

        // GET: /Checkout
        [HttpGet]
        public IActionResult Index()
        {
            var items = _cart.GetCartItems();
            if (items == null || !items.Any())
                return RedirectToAction("Products", "Shop", new { shopId = 1 });

            var subtotal = items.Sum(i => i.Product.Price * i.Quantity);

            var viewModel = new CheckoutViewModel
            {
                CartItems = items,
                SubTotal = subtotal,
                Discount = 0,
                TotalUSD = subtotal,
                DeliveryAddress = "",
                RequestChangeOption = "None",
                AvailableDrivers = _context.Drivers.Include(d => d.User).ToList() // 👈 Load available drivers
            };

            return View(viewModel);
        }

        // POST: /Checkout/ApplyPromo
        [HttpPost]
        public IActionResult ApplyPromo(CheckoutViewModel model)
        {
            var items = _cart.GetCartItems();
            if (items == null || !items.Any())
                return RedirectToAction(nameof(Index));

            model.CartItems = items;
            model.SubTotal = items.Sum(i => i.Product.Price * i.Quantity);

            if (!string.IsNullOrEmpty(model.PromoCode) && model.PromoCode.ToUpper() == "LUCKY")
            {
                model.Discount = model.SubTotal * 0.10m;
            }
            else
            {
                model.Discount = 0;
                ModelState.AddModelError("PromoCode", "❌ Invalid promo code.");
            }

            model.TotalUSD = model.SubTotal - model.Discount;
            model.AvailableDrivers = _context.Drivers.Include(d => d.User).ToList(); // Needed to re-render driver list

            return View("Index", model);
        }

        // POST: /Checkout (finalize order)
        [HttpPost]
        public IActionResult Index(CheckoutViewModel model)
        {
            var items = _cart.GetCartItems();
            if (items == null || !items.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                model.AvailableDrivers = _context.Drivers.Include(d => d.User).ToList();
                return View(model);
            }

            // ✅ Get client ID from session
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            decimal subtotal = items.Sum(i => i.Product.Price * i.Quantity);
            decimal discount = (!string.IsNullOrEmpty(model.PromoCode) && model.PromoCode.ToUpper() == "LUCKY")
                ? subtotal * 0.10m
                : 0;

            decimal total = subtotal - discount;

            var order = new ShopOrder
            {
                ClientId = userId,
                ShopId = items.First().Product.ShopId,
                OrderDate = DateTime.Now,
                DeliveryAddress = model.DeliveryAddress,
                RequestChange = model.RequestChangeOption,
                TotalAmountUSD = total,
                TotalAmountLBP = total * 90000,
                PromoCode = model.PromoCode,
                EstimatedDeliveryTime = "20 - 40 minutes",
                Status = OrderStatus.Pending,
                AssignedDriverId = model.SelectedDriverId, // 👈 Save selected driver
                Items = items.Select(i => new ShopOrderItem
                {
                    ProductId = i.Product.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            _context.ShopOrders.Add(order);
            _context.SaveChanges();
            _cart.ClearCart();

            return RedirectToAction(nameof(OrderConfirmation), new { shopOrderId = order.ShopOrderId });
        }

        // GET: /Checkout/OrderConfirmation
        public IActionResult OrderConfirmation(int shopOrderId)
        {
            var order = _context.ShopOrders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.ShopOrderId == shopOrderId);

            if (order == null)
                return NotFound();

            // 🧠 Load assigned driver with user info
            Driver driver = null;
            if (order.AssignedDriverId.HasValue)
            {
                driver = _context.Drivers
                    .Include(d => d.User)
                    .FirstOrDefault(d => d.DriverId == order.AssignedDriverId.Value);
            }

            decimal subtotal = order.Items.Sum(i => i.Product.Price * i.Quantity);
            decimal discount = (!string.IsNullOrEmpty(order.PromoCode) && order.PromoCode.ToUpper() == "LUCKY")
                ? subtotal * 0.10m
                : 0;

            var vm = new OrderConfirmationViewModel
            {
                ShopOrderId = order.ShopOrderId,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                RequestChange = order.RequestChange,
                TotalAmountUSD = order.TotalAmountUSD,
                TotalAmountLBP = order.TotalAmountLBP,
                PromoCode = order.PromoCode,
                EstimatedDeliveryTime = order.EstimatedDeliveryTime,
                Status = order.Status,
                Discount = discount,
                AssignedDriver = driver,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Name = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Product.Price
                }).ToList()
            };

            return View(vm);
        }
    }
}
