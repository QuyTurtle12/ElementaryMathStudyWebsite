using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.CheckoutPages
{
    public class StudentSelectModel : PageModel
    {
        private readonly IAppUserServices _userService;
        public StudentSelectModel(IAppUserServices userService)
        {
            _userService = userService;
        }

        public BasePaginatedList<User> myChildren = default!;

		[BindProperty]
		public string? SelectedStudentId { get; set; }  // This will hold the selected subject's ID from the form

		public async Task<IActionResult> OnGetAsync()
        {
            string? parentId = HttpContext.Session.GetString("user_id");
            if (string.IsNullOrWhiteSpace(parentId))
            {
                return Content("");
            }

            myChildren = await _userService.GetChildrenOfParentAsync(parentId, -1, -1);

            return Page();
        }

		public IActionResult OnPostAsync()
        {
			if (string.IsNullOrWhiteSpace(SelectedStudentId)) { 
				TempData["ErrorMessage"] = "No subject selected.";
				return Page();  // You can return an error message or stay on the page
			}

			HttpContext.Session.SetString("selected_student_id", SelectedStudentId);

			return Redirect("/PurchasePages/CourseSelect");
        }

	}
}
