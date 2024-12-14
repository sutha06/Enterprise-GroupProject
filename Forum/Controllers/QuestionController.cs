using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Forum.Data;
using Forum.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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
                .Include(q => q.User)
                .Include(q => q.Answers);

            return View(await questions.ToListAsync());
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var question = await _context.Questions
                .Include(q => q.User)

            if (question == null)
                return NotFound();

            return View(question);
        }

        // GET: Questions/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Questions/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Question question)
        {
            if (ModelState.IsValid)
            {
                question.IdentityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            if (ModelState.IsValid)
            {
                answer.IdentityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Answers.Add(answer);
                await _context.SaveChangesAsync();

                var question = await _context.Questions
                    .Include(q => q.User)
                    .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                    .FirstOrDefaultAsync(q => q.Id == answer.QuestionId);

                if (question != null)
                    return View("Details", question);
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

            if (question == null || question.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
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
            if (existingQuestion == null || existingQuestion.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
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

       
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null || question.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Unauthorized();

            return View(question);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null || question.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
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
