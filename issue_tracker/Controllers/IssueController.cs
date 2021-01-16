using System.Threading.Tasks;
using issue_tracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace issue_tracker.Controllers
{
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
    }
}