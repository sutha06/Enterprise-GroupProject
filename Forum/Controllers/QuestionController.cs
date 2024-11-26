using Microsoft.AspNetCore.Mvc;
using Forum.Data;
using Forum.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Forum.Controllers;

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
        var applicationDbContext = _context.Questions.Include(q => q.User).Include(a=>a.Answers);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: Questions/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Questions == null)
        {
            return NotFound();
        }

        var question = await _context.Questions
            .Include(q => q.User)         // Include User
            .Include(q => q.Answers)      // Include Answers
            .ThenInclude(a => a.User)     // Include User for each Answer
            .FirstOrDefaultAsync(m => m.Id == id);

        if (question == null)
        {
            return NotFound();
        }

        return View(question);
    }



    // GET: Questions/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Questions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Description,IdentityUserId")] Question question)
    {
        if (ModelState.IsValid)
        {
            _context.Add(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
            
        return View(question);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAnswer([Bind("Id,Content,QuestionId,IdentityUserId")] Answer answer)
    {
        if (ModelState.IsValid)
        {
            // Add answer to the database
            _context.Add(answer);
            await _context.SaveChangesAsync();
        }

        // Fetch the question again to include updated answers
        var question = await _context.Questions
            .Include(q => q.User)
            .Include(q => q.Answers)
            .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(q => q.Id == answer.QuestionId);

        if (question == null)
        {
            return NotFound();
        }

        return View("Details", question);
    }


    // GET: Questions/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var question = await _context.Questions.FindAsync(id);
        if (question == null) return NotFound();
        return View(question);
    }

    // POST: Questions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Question question)
    {
        if (id != question.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(question);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(question.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(question);
    }

    // GET: Questions/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var question = await _context.Questions
            .FirstOrDefaultAsync(m => m.Id == id);
        if (question == null) return NotFound();

        return View(question);
    }

    // POST: Questions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool QuestionExists(int id)
    {
        return _context.Questions.Any(e => e.Id == id);
    }
}