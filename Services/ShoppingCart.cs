using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DeliveryAppSystem.Models;
using DeliveryAppSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryAppSystem.Services
{
    public class ShoppingCart
    {
        private readonly ApplicationDbContext _context;

        public string ShoppingCartId { get; set; }

        public ShoppingCart(ApplicationDbContext context)
        {
            _context = context;
        }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>().HttpContext.Session;
            var context = services.GetService<ApplicationDbContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Product product, int quantity)
        {
            var cartItem = _context.ShoppingCartItems.FirstOrDefault(
                s => s.ProductId == product.ProductId && s.ShoppingCartId == ShoppingCartId);

            if (cartItem == null)
            {
                cartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    ProductId = product.ProductId,
                    Quantity = quantity
                };
                _context.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            _context.SaveChanges();
        }

        public void AddToCart(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                AddToCart(product, 1);
            }
        }

        public void DecreaseQuantity(int productId)
        {
            var cartItem = _context.ShoppingCartItems
                .SingleOrDefault(c => c.ShoppingCartId == ShoppingCartId && c.ProductId == productId);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    _context.ShoppingCartItems.Remove(cartItem);
                }
                _context.SaveChanges();
            }
        }

        public List<ShoppingCartItem> GetCartItems()
        {
            return _context.ShoppingCartItems
                .Where(c => c.ShoppingCartId == ShoppingCartId)
                .Include(c => c.Product)
                .ToList();
        }

        public void ClearCart()
        {
            var items = _context.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId);
            _context.ShoppingCartItems.RemoveRange(items);
            _context.SaveChanges();
        }
    }
}
