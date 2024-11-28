using Microsoft.AspNetCore.Mvc;
using Forum.Data;
using Forum.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Forum.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            var questions = _context.Questions
                .Include(q => q.Profile)  // Include the Profile navigation property
                .Include(q => q.Answers);

            return View(await questions.ToListAsync());
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var question = await _context.Questions
                .Include(q => q.Profile)  // Include the Profile navigation property
                .Include(q => q.Answers)
                .ThenInclude(a => a.Profile)  // Include Profile in answers too
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
                return NotFound();

            return View(question); // Ensure you pass a single Question, not a list
        }

        // GET: Questions/Create
        [Authorize]
        public IActionResult Create()
        {
            // Ensure user is logged in by checking session
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // Redirect to login if no email is found in the session
                return RedirectToAction("Login", "Profile");
            }

            return View();
        }

        // POST: Questions/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Question question)
        {
            // Ensure user is logged in by checking session
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // Redirect to login if no email is found in the session
                return RedirectToAction("Login", "Profile");
            }

            var userProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == userEmail);

            if (userProfile == null)
            {
                // Handle case where the user profile does not exist
                return RedirectToAction("Login", "Profile");
            }

            if (ModelState.IsValid)
            {
                // Save the question and associate it with the logged-in user
                question.ProfileId = userProfile.Id;  // Use ProfileId to associate the question with the user
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(question);
        }

        // POST: Questions/AddAnswer
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnswer([Bind("Content,QuestionId")] Answer answer)
        {
            // Ensure user is logged in by checking session
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // Redirect to login if no email is found in the session
                return RedirectToAction("Login", "Profile");
            }

            var userProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == userEmail);

            if (userProfile == null)
            {
                // Handle case where the user profile does not exist
                return RedirectToAction("Login", "Profile");
            }

            if (ModelState.IsValid)
            {
                // Save the answer and associate it with the logged-in user
                answer.ProfileId = userProfile.Id;  // Use ProfileId to associate the answer with the user
                _context.Answers.Add(answer);
                await _context.SaveChangesAsync();

                var question = await _context.Questions
                    .Include(q => q.Profile)
                    .Include(q => q.Answers)
                    .ThenInclude(a => a.Profile)
                    .FirstOrDefaultAsync(q => q.Id == answer.QuestionId);

                if (question != null)
                    return View("Details", question);  // Redirect to the Details view of the same question
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Questions/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var question = await _context.Questions.FindAsync(id);

            if (question == null)
                return NotFound();

            // Ensure the question belongs to the logged-in user
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == userEmail);

            if (userProfile == null || question.ProfileId != userProfile.Id)  // Check ProfileId instead of UserId
                return Unauthorized();

            return View(question);
        }

        // POST: Questions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description")] Question question)
        {
            if (id != question.Id)
                return NotFound();

            var existingQuestion = await _context.Questions.FindAsync(id);
            if (existingQuestion == null)
                return NotFound();

            // Ensure the question belongs to the logged-in user
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == userEmail);

            if (userProfile == null || existingQuestion.ProfileId != userProfile.Id)  // Check ProfileId instead of UserId
                return Unauthorized();

            if (ModelState.IsValid)
            {
                try
                {
                    existingQuestion.Title = question.Title;
                    existingQuestion.Description = question.Description;
                    _context.Update(existingQuestion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(id))
                        return NotFound();

                    throw;
                }
            }

            return View(question);
        }

        // GET: Questions/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
                return NotFound();

            // Ensure the question belongs to the logged-in user
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == userEmail);

            if (userProfile == null || question.ProfileId != userProfile.Id)  // Check ProfileId instead of UserId
                return Unauthorized();

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
                return NotFound();

            // Ensure the question belongs to the logged-in user
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == userEmail);

            if (userProfile == null || question.ProfileId != userProfile.Id)  // Check ProfileId instead of UserId
                return Unauthorized();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(q => q.Id == id);
        }
    }
}
