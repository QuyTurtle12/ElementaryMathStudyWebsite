using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.SubjectPages
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseContext _context;

        public IndexModel(DatabaseContext context)
        {
            _context = context;
        }

        public IList<Subject> Subject { get; set; } = default!;
        public string UserRole { get; private set; } = string.Empty;
        public bool IsLoggedIn { get; private set; } = false;
        public bool IsValidRole { get; private set; } = false;

        [BindProperty(SupportsGet = true)]
        public string SearchName { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public double? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public double? MaxPrice { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve session information
            var userId = HttpContext.Session.GetString("user_id");
            UserRole = HttpContext.Session.GetString("role_name");
            IsLoggedIn = !string.IsNullOrEmpty(userId);

            // Validate role
            IsValidRole = UserRole == "Admin" || UserRole == "Content Manager" || UserRole == "Manager";

            // Fetch all subjects
            var query = _context.Subject.AsQueryable();

            // Apply filters based on search criteria
            if (!string.IsNullOrEmpty(SearchName))
            {
                query = query.Where(s => EF.Functions.Like(s.SubjectName, $"%{SearchName}%"));
            }
            if (MinPrice.HasValue)
            {
                query = query.Where(s => s.Price >= MinPrice.Value);
            }
            if (MaxPrice.HasValue)
            {
                query = query.Where(s => s.Price <= MaxPrice.Value);
            }

            var subjects = await query.ToListAsync();

            // Collect all unique user IDs from the subjects
            var userIds = subjects
                .SelectMany(s => new[] { s.CreatedBy, s.LastUpdatedBy, s.DeletedBy })
                .Where(id => !string.IsNullOrEmpty(id))
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

            Subject = subjects;
            return Page();
        }
    }
}