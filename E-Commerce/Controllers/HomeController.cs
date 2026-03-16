using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Data;

namespace E_Commerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? categoryId)
        {
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();

            var productsQuery = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);

            var products = productsQuery.OrderByDescending(p => p.CreatedAt).ToList();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.ProductCounts = _context.Products
                .GroupBy(p => p.CategoryId)
                .ToDictionary(g => g.Key, g => g.Count());

            return View(products);
        }
    }
}