using DeliveryAppSystem.Data;
using DeliveryAppSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    //public IActionResult Menu()
    //{
    //    var categories = _context.ProductCategories
    //        .Include(c => c.Products)
    //        .ToList();

    //    return View(categories);
    //}
}
