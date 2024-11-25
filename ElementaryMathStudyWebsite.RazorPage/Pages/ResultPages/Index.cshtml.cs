using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ResultPages
{
    public class IndexModel : PageModel
    {
        private readonly IAppUserServices _userService;

        public IndexModel(IAppUserServices userService)
        {
            _userService = userService;
        }

        public IList<Result> Result { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            string currentUserId = HttpContext.Session.GetString("user_id")!;
            User? currentUser = await _userService.GetUserByIdAsync(currentUserId);

            if (currentUser!.Role!.RoleName.Equals("Student"))
            {
                return RedirectToPage("ResultPages/Student");
            }
            //else if (currentUser!.Role!.RoleName.Equals("Parent"))
            //{
            //    return RedirectToPage("/ResultPages/Parent");
            //}

            return RedirectToPage("/AccessDenied");
        }

    }
}
