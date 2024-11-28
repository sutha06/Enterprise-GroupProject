using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace Forum.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation("Register GET action accessed.");
            return View();
        }

        // Registration (Create Profile)
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Register POST action triggered.");

            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (await _context.Profiles.AnyAsync(p => p.Email == model.Email))
                {
                    _logger.LogWarning("Email already registered.");
                    ModelState.AddModelError("", "This email is already registered.");
                    return View(model);
                }

                // Hash the password before storing it
                var hashedPassword = HashPassword(model.Password);

                // Generate a unique UserId for the new profile
                var userId = Guid.NewGuid().ToString();

                // Save profile in database
                var profile = new Profile
                {
                    Email = model.Email,
                    Password = hashedPassword,
                    UserId = userId // Assign the generated UserId
                };

                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Profile registered successfully.");

                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Login GET action accessed.");
            return View();
        }

        // Login (Authenticate Profile)
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation("Login POST action triggered.");

            if (ModelState.IsValid)
            {
                // Find profile by email
                var profile = await _context.Profiles
                    .FirstOrDefaultAsync(p => p.Email == model.Email);

                if (profile != null && VerifyPassword(model.Password, profile.Password))
                {
                    _logger.LogInformation("Login successful. Setting session.");

                    // Set session for user
                    HttpContext.Session.SetString("UserEmail", profile.Email);

                    TempData["SuccessMessage"] = "Login successful! Welcome back.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning("Invalid email or password.");
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }

            return View(model);
        }

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _logger.LogInformation("Logout action triggered.");

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Utility: Hash Password
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // Utility: Verify Password
        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            var hashedInput = HashPassword(inputPassword);
            return hashedInput == storedHashedPassword;
        }
    }
}
