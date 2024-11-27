using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.SubjectPages
{
    public class DetailsModel : PageModel
    {
        private readonly DatabaseContext _context;

        public DetailsModel(DatabaseContext context)
        {
            _context = context;
        }

        public Subject Subject { get; set; } = default!;
        public string? CreatedByName { get; set; }
        public string? LastUpdatedByName { get; set; }
        public string? DeletedByName { get; set; }

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

            // Fetch user names
            var userIds = new[] { subject.CreatedBy, subject.LastUpdatedBy, subject.DeletedBy }
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            var users = await _context.User
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.FullName);

            // Map IDs to user names and add DeletedTime information
            CreatedByName = !string.IsNullOrEmpty(subject.CreatedBy)
                ? $"{users.GetValueOrDefault(subject.CreatedBy, "Unknown")} (Deleted: {(subject.DeletedTime.HasValue ? subject.DeletedTime.Value.ToString() : "None")})"
                : "Unknown";

            LastUpdatedByName = !string.IsNullOrEmpty(subject.LastUpdatedBy)
                ? $"{users.GetValueOrDefault(subject.LastUpdatedBy, "Unknown")} (Deleted: {(subject.DeletedTime.HasValue ? subject.DeletedTime.Value.ToString() : "None")})"
                : "Unknown";

            DeletedByName = !string.IsNullOrEmpty(subject.DeletedBy)
                ? $"{users.GetValueOrDefault(subject.DeletedBy, "Unknown")} (Deleted: {(subject.DeletedTime.HasValue ? subject.DeletedTime.Value.ToString() : "None")})"
                : "Unknown";

            Subject = subject;
            return Page();
        }
    }
}
