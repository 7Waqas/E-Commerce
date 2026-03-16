using Microsoft.AspNetCore.Mvc;
using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private bool IsAdminLoggedIn() =>
            HttpContext.Session.GetString("IsAdmin") == "true";

        // ── Dashboard ────────────────────────────────────────────────
        public IActionResult Dashboard()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");
            var products = _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            return View(products);
        }

        // ── Create GET ───────────────────────────────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");
            ViewBag.Categories = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name");
            return View();
        }

        // ── Create POST ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");

            // Remove ImagePath from validation (we set it manually)
            ModelState.Remove("ImagePath");
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                // Handle image upload
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    product.ImagePath = await SaveImageAsync(product.ImageFile);
                }

                product.CreatedAt = DateTime.Now;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"✓ '{product.Title}' added successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(product);
        }

        // ── Edit GET ─────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            ViewBag.Categories = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ── Edit POST ────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");

            ModelState.Remove("ImagePath");
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                // Fetch existing product to preserve old image if no new one uploaded
                var existing = _context.Products.Find(product.Id);
                if (existing == null) return NotFound();

                existing.Title = product.Title;
                existing.Description = product.Description;
                existing.ProductUrl = product.ProductUrl;

                // Only replace image if admin uploaded a new one
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    // Delete old image file from disk
                    DeleteOldImage(existing.ImagePath);
                    existing.ImagePath = await SaveImageAsync(product.ImageFile);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"✓ '{existing.Title}' updated successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(product);
        }

        // ── Delete POST ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login", "Admin");

            var product = _context.Products.Find(id);
            if (product != null)
            {
                DeleteOldImage(product.ImagePath);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"✓ '{product.Title}' deleted.";
            }

            return RedirectToAction("Dashboard");
        }

        // ── Helpers ──────────────────────────────────────────────────

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            // Create uploads folder if it doesn't exist
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Create unique filename to avoid collisions
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + uniqueFileName;
        }

        private void DeleteOldImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}