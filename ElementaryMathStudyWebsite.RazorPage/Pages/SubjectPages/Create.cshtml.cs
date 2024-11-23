using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Services;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Services.Service;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.SubjectPages
{
    public class CreateModel : PageModel
    {
        private readonly DatabaseContext _context;

        public CreateModel(DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Subject Subject { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Perform auditing
            await AuditFields(Subject, isCreating: true);

            // Add the new subject to the database
            _context.Subject.Add(Subject);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task AuditFields(BaseEntity entity, bool isCreating = false)
        {
            // Get the current logged-in user's ID
            string currentUserId = HttpContext.Session.GetString("user_id") ?? "Unknown";

            if (isCreating)
            {
                entity.CreatedBy = currentUserId.ToUpper();
                entity.CreatedTime = DateTime.UtcNow; // Set the creation time
            }

            entity.LastUpdatedBy = currentUserId.ToUpper();
            entity.LastUpdatedTime = DateTime.UtcNow; // Always update this field
        }
    }
}
