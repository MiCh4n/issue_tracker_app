using System;
using System.Threading.Tasks;
using issue_tracker.Data;
using issue_tracker.Models;
using issue_tracker.Views.Issue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace issue_tracker.Controllers
{
    [Authorize]
    public class IssueController : Controller
    {
        private readonly IssueContext _context;
        private readonly UserManager<ApplicationUser> _currentUser;

        public IssueController(IssueContext context,
            UserManager<ApplicationUser> currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
    
        public async Task<IActionResult> Data()
        {
            return View(await _context.Issues.ToListAsync());
        }
        //GET      issue create
         public IActionResult Create()
                {
                    return View();
                }
         //POST    issue create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title, Description, Priority")] Issue issue)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(issue);
                    issue.Phase = Phase.todo;
                    issue.AddDate = DateTime.Now;
                    issue.AuthorId = _currentUser.GetUserId(User);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Data));
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes.");
            }
            return View(issue);
        }
        //GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }
            return View(issue);
        }
        //POST: Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var issueToUpdate = await _context.Issues.FirstOrDefaultAsync(i => i.ID == id);
            if (await TryUpdateModelAsync<Issue>(
                issueToUpdate,
                "",
                i => i.Title, i => i.Description))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Data));
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes.");
                }
            }
            return View(issueToUpdate);
        }
        
        // GET: Delete
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == id);
            if (issue == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed.";
            }

            return View(issue);
        }
        
        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return RedirectToAction(nameof(Data));
            }

            try
            {
                _context.Issues.Remove(issue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Data));
            }
            catch (DbUpdateException /* ex */)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        public async Task<IActionResult> Take(int? id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return RedirectToAction(nameof(Data));
            }
            return RedirectToAction(nameof(Data));
        }
    }
}