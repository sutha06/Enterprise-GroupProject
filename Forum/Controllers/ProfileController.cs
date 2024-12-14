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

        using Microsoft.AspNetCore.Identity;

[HttpPost]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    _logger.LogInformation("Register POST action triggered.");

    if (ModelState.IsValid)
    {
      
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Email already registered in AspNetUsers.");
            ModelState.AddModelError("", "This email is already registered.");
            return View(model);
        }

       //
        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var createResult = await _userManager.CreateAsync(user, model.Password);
        if (createResult.Succeeded)
        {
          
            var profile = new Profile
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id 
            };

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();
//
            _logger.LogInformation("Profile registered and linked to AspNetUsers successfully.");

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }
        else
        {
            foreach (var error in createResult.Errors)
            {
                _logger.LogWarning($"Identity error: {error.Description}");
                ModelState.AddModelError("", error.Description);
            }
        }
    }

    return View(model);
}

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Login GET action accessed.");
            return View();
        }

       
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation("Login POST action triggered.");

            if (ModelState.IsValid)
            {
                
                var profile = await _context.Profiles
                    .FirstOrDefaultAsync(p => p.Email == model.Email);

                if (profile != null && VerifyPassword(model.Password, profile.Password))
                {
                    _logger.LogInformation("Login successful. Setting session.");

                    
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

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _logger.LogInformation("Logout action triggered.");

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

       
        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            var hashedInput = HashPassword(inputPassword);
            return hashedInput == storedHashedPassword;
        }
    }
}
