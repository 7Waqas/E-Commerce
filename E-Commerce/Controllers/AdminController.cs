using Microsoft.AspNetCore.Mvc;
using E_Commerce.Models;

namespace E_Commerce.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (HttpContext.Session.GetString("IsAdmin") == "true")
                return RedirectToAction("Dashboard", "Product");

            return View();
        }

        [HttpPost]
        public IActionResult Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var correctUsername = _configuration["AdminCredentials:Username"];
            var correctPassword = _configuration["AdminCredentials:Password"];

            if (model.Username == correctUsername && model.Password == correctPassword)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Dashboard", "Product");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}