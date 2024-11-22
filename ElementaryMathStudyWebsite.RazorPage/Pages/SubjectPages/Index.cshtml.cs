using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.SubjectPages
{
    public class IndexModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public IndexModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public IList<Subject> Subject { get;set; } = default!;

        public async Task OnGetAsync()
        {
            // Fetch all subjects
            var subjects = await _context.Subject.ToListAsync();

            // Collect all unique user IDs (ignore nulls)
            var userIds = subjects
                .SelectMany(s => new[] { s.CreatedBy, s.LastUpdatedBy, s.DeletedBy })
                .Where(id => !string.IsNullOrEmpty(id)) // Ensure IDs are not null or empty
                .Distinct()
                .ToList();

            // Query the User table to get user names
            var users = await _context.User
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.FullName);

            // Replace IDs with user names
            foreach (var subject in subjects)
            {
                subject.CreatedBy = string.IsNullOrEmpty(subject.CreatedBy)
                    ? "Unknown"
                    : users.GetValueOrDefault(subject.CreatedBy, "Unknown");

                subject.LastUpdatedBy = string.IsNullOrEmpty(subject.LastUpdatedBy)
                    ? "Unknown"
                    : users.GetValueOrDefault(subject.LastUpdatedBy, "Unknown");

                subject.DeletedBy = string.IsNullOrEmpty(subject.DeletedBy)
                    ? "Unknown"
                    : users.GetValueOrDefault(subject.DeletedBy, "Unknown");
            }

            // Assign to the model property
            Subject = subjects;
        }
    }
}
