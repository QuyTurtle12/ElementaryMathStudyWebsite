using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Services;
using ElementaryMathStudyWebsite.Services.Service;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.SubjectPages
{
    public class EditModel : PageModel
    {
        private readonly DatabaseContext _context;

        public EditModel(DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Subject Subject { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subject.FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            Subject = subject;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Additional server-side validation
            if (Subject.Price <= 0)
            {
                ModelState.AddModelError("Subject.Price", "Price must be a positive integer.");
                return Page();
            }

            var existingSubject = await _context.Subject.FindAsync(Subject.Id);
            if (existingSubject == null)
            {
                return NotFound();
            }

            // Update fields
            existingSubject.SubjectName = Subject.SubjectName;
            existingSubject.Price = Subject.Price;
            existingSubject.Status = Subject.Status;

            // Update audit fields
            // Get the current logged-in user's ID
            string currentUserId = HttpContext.Session.GetString("user_id") ?? "Unknown";
            existingSubject.LastUpdatedBy = currentUserId.ToUpper();
            existingSubject.LastUpdatedTime = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(Subject.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool SubjectExists(string id)
        {
            return _context.Subject.Any(e => e.Id == id);
        }
    }
}
