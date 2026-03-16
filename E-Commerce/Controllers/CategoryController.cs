using Microsoft.AspNetCore.Mvc;
using E_Commerce.Data;
using E_Commerce.Models;

namespace E_Commerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdminLoggedIn() =>
            HttpContext.Session.GetString("IsAdmin") == "true";

        // ── List all categories ──────────────────────────────────────
        public IActionResult Index()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");

            var categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            // Attach product count to ViewBag
            ViewBag.ProductCounts = _context.Products
                .GroupBy(p => p.CategoryId)
                .ToDictionary(g => g.Key, g => g.Count());

            return View(categories);
        }

        // ── Create GET ───────────────────────────────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");
            return View();
        }

        // ── Create POST ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");

            ModelState.Remove("Products");

            if (ModelState.IsValid)
            {
                // Check duplicate name
                if (_context.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower()))
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(category);
                }

                category.CreatedAt = DateTime.Now;
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["Success"] = $"✓ Category '{category.Name}' created!";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // ── Edit GET ─────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // ── Edit POST ────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");

            ModelState.Remove("Products");

            if (ModelState.IsValid)
            {
                // Check duplicate name (exclude self)
                if (_context.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != category.Id))
                {
                    ModelState.AddModelError("Name", "A category with this name already exists.");
                    return View(category);
                }

                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["Success"] = $"✓ Category '{category.Name}' updated!";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // ── Delete POST ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");

            var category = _context.Categories.Find(id);
            if (category != null)
            {
                // Check if any products use this category
                bool hasProducts = _context.Products.Any(p => p.CategoryId == id);
                if (hasProducts)
                {
                    TempData["Error"] = "⚠ Cannot delete — products exist in this category. Reassign them first.";
                    return RedirectToAction("Index");
                }

                _context.Categories.Remove(category);
                _context.SaveChanges();
                TempData["Success"] = $"✓ Category '{category.Name}' deleted.";
            }

            return RedirectToAction("Index");
        }
    }
}