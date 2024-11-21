using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ProgressPages
{
    [Authorize(Policy = "Student")]
    public class IndexModel : PageModel
    {
		private readonly IAppUserServices _userService;
		private readonly IAppProgressServices _progressService;

		public IndexModel(IAppUserServices userService, IAppProgressServices progressService)
		{
			_userService = userService;
			_progressService = progressService;
		}

		public BasePaginatedList<ProgressViewDto>? Progresses { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
			string currentUserId = HttpContext.Session.GetString("user_id")!;
			User? currentUser = await _userService.GetUserByIdAsync(currentUserId);
			if (currentUser == null)
			{
				return Unauthorized();
			}

			Progresses = await _progressService.GetStudentProgressesDtoForStudentAsync(pageNumber, pageSize, currentUser);
			return Page();
		}
    }
}
