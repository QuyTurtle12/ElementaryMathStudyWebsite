using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.AuthPages
{
    public class ProfileModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public ProfileModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public new User User { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync()
        {
            string id = HttpContext.Session.GetString("user_id") ?? "";

            if (string.IsNullOrWhiteSpace(id)) // Check if user_id is set in the session
            {
                return RedirectToPage("/AuthPages/LoginError"); // Redirect to login if session is missing
            }
            

            var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return RedirectToPage("/AuthPages/LoginError");
            }
            else
            {
                User = user;
            }

            return Page();
        }
    }
}
