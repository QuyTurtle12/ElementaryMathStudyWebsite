using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserPages
{
    [Authorize(Policy = "Parent")]
    public class ParentModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public ParentModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public new IList<User> User { get; set; } = default!;

        public async Task OnGetAsync()
        {
            string parentId = HttpContext.Session.GetString("user_id") ?? "";

            if (string.IsNullOrWhiteSpace(parentId)) // Check if user_id is set in the session
            {
                RedirectToPage("/AuthPages/LoginError"); // Redirect to login if session is missing
            }
            User = await _context.User.Where(u => u.CreatedBy == parentId && u.Id != parentId)
                .ToListAsync();
        }
    }
}
