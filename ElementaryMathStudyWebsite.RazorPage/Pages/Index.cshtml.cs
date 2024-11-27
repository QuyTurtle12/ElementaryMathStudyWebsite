using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElementaryMathStudyWebsite.RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public JsonResult OnGetSessionInfo()
        {
            var userId = HttpContext.Session.GetString("user_id");
            var roleId = HttpContext.Session.GetString("role_id");

            // Return session info as JSON
            return new JsonResult(new { userId, roleId });
        }

        public IActionResult OnPostLogout()
        {
            // Clear session data
            HttpContext.Session.Clear();
            return RedirectToPage("AuthPages/Login"); // Redirect to login page
        }
    }
}
