using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using Microsoft.Extensions.Logging; // Add this for logging
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Forum.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ProfileController> _logger; // Logger

        public ProfileController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ProfileController> logger) // Inject the logger
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger; // Set logger
        }

        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation("Register GET action hit.");
            return View();
        }

        // Registration (Create Profile)
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Register POST action hit with model: {model}", model.Email);

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model is valid. Attempting to create user.");

                // Create a new IdentityUser
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };

                // Attempt to create the user in AspNetUsers
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created successfully, now saving profile.");

                    // Save the profile data linked with the AspNetUser ID
                    var profile = new Profile
                    {
                        UserId = user.Id,
                        Email = model.Email,
                        Password = model.Password // Password field for additional profile info
                    };
                    _context.Add(profile);
                    await _context.SaveChangesAsync();

                    // Log in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Store success message in TempData
                    TempData["SuccessMessage"] = "Registration successful! Welcome to the forum.";

                    _logger.LogInformation("Registration successful. Redirecting to Home.");

                    return RedirectToAction("Index", "Home");
                }

                // Add Identity creation errors to ModelState
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("Identity error: {error}", error.Description);
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Login GET action hit.");
            return View();
        }

        // Login (Authenticate Profile)
        [HttpPost]
public async Task<IActionResult> Login(LoginViewModel model)
{
    _logger.LogInformation("Login POST action hit with model: {model}", model.Email);

    if (ModelState.IsValid)
    {
        _logger.LogInformation("Model is valid. Attempting to find profile.");

        // Look for the profile in the Profiles table (since you are saving profile data in the database)
        var profile = await _context.Profiles
                                     .FirstOrDefaultAsync(p => p.Email == model.Email && p.Password == model.Password);

        if (profile != null)
        {
            _logger.LogInformation("Profile found. Attempting to sign in the user.");

            // Use SignInManager to sign in the corresponding AspNetUser
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Login successful. Redirecting to Home.");

                    // Redirect user to the home page
                    TempData["SuccessMessage"] = "Login successful! Welcome back!";
                    return RedirectToAction("Index", "Home"); // Redirecting to Home page
                }
                else
                {
                    _logger.LogWarning("Invalid login attempt for user: {email}", model.Email);
                    // Add error message for invalid login attempt
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }
            }
            else
            {
                _logger.LogWarning("User not found: {email}", model.Email);
                // Handle user not found
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }
        }
        else
        {
            _logger.LogWarning("Profile not found for user: {email}", model.Email);
            // Handle profile not found
            ModelState.AddModelError("", "Profile not found.");
            return View(model);
        }
    }
    else
    {
        _logger.LogWarning("Model is invalid. Returning to the view.");
    }

    // Return the view only if login failed or model is invalid
    return View(model);
}


        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout action hit. Signing out.");

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
