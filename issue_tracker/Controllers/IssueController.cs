using System;
using System.Threading.Tasks;
using issue_tracker.Data;
using issue_tracker.Models;
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

        [Authorize(Roles = "admin")]
        public IActionResult AdminPanel()
        {
            var users = _currentUser.Users;
            return View(users);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignRole(string id)
        {
            if (id == null) return NotFound();
            var user = await _currentUser.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ActionName("AssignRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRolePost(string id)
        {
            if (id == null) return NotFound();
            var userToUpdate = await _currentUser.FindByIdAsync(id);
            try
            {
                var role = Request.Form["role"];
                if (await _currentUser.IsInRoleAsync(userToUpdate, "admin"))
                {
                    await _currentUser.RemoveFromRoleAsync(userToUpdate, "admin");
                    await _context.SaveChangesAsync();
                }

                if (await _currentUser.IsInRoleAsync(userToUpdate, "developer"))
                {
                    await _currentUser.RemoveFromRoleAsync(userToUpdate, "developer");
                    await _context.SaveChangesAsync();
                }

                if (await _currentUser.IsInRoleAsync(userToUpdate, "user"))
                {
                    await _currentUser.RemoveFromRoleAsync(userToUpdate, "user");
                    await _context.SaveChangesAsync();
                }

                await _currentUser.AddToRoleAsync(userToUpdate, role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminPanel));
            }
            catch
            {
                ModelState.AddModelError("", "Unable to save changes.");
            }

            return View(userToUpdate);
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
                    issue.AuthorName = _currentUser.GetUserName(User);
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
            if (id == null) return NotFound();

            var issue = await _context.Issues.FindAsync(id);
            if (issue == null || issue.AuthorId != _currentUser.GetUserId(User)) return NotFound();

            return View(issue);
        }

        //POST: Edit
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null) return NotFound();

            var issueToUpdate = await _context.Issues.FirstOrDefaultAsync(i => i.ID == id);
            if (await TryUpdateModelAsync(
                issueToUpdate,
                "",
                i => i.Title, i => i.Description))
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Data));
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes.");
                }

            return View(issueToUpdate);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null) return NotFound();

            var issue = await _context.Issues
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == id);
            if (issue == null || issue.AuthorId != _currentUser.GetUserId(User)) return NotFound();

            if (saveChangesError.GetValueOrDefault())
                ViewData["ErrorMessage"] =
                    "Delete failed.";

            return View(issue);
        }

        // POST: Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null || issue.Phase != Phase.todo) return RedirectToAction(nameof(Data));

            try
            {
                _context.Issues.Remove(issue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Data));
            }
            catch (DbUpdateException /* ex */)
            {
                return RedirectToAction(nameof(Delete), new {id, saveChangesError = true});
            }
        }

        [Authorize(Roles = "developer, admin")]
        public async Task<IActionResult> Take(int? id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null) return RedirectToAction(nameof(Data));

            try
            {
                _context.Issues.Update(issue);
                issue.Phase = Phase.inprogress;
                issue.ReviewerId = _currentUser.GetUserId(User);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Data));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Data));
            }
        }

        [Authorize(Roles = "developer, admin")]
        public async Task<IActionResult> Close(int? id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null) return RedirectToAction(nameof(Data));

            try
            {
                _context.Issues.Update(issue);
                issue.Phase = Phase.done;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Data));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Data));
            }
        }
    }
}