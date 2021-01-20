using System;
using System.Threading.Tasks;
using issue_tracker.Data;
using issue_tracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace issue_tracker.Controllers
{
    [Authorize]
    public class IssueController : Controller
    {
        private readonly IssueContext _context;

        public IssueController(IssueContext context)
        {
            _context = context;
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
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Data));
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }
            return View(issue);
        }
    }
}