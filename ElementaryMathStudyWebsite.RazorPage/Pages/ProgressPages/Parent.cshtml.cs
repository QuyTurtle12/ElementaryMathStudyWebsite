using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ProgressPages
{
    [Authorize(Policy = "Parent")]
    public class ParentModel : PageModel
    {
        private readonly IAppUserServices _userService;
        private readonly IAppProgressServices _progressService;

        public ParentModel(IAppProgressServices progressService, IAppUserServices userService)
        {
            _progressService = progressService;
            _userService = userService;
        }

        public BasePaginatedList<ProgressViewDto>? Progresses { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string studentId, int pageNumber = 1, int pageSize = 5)
        {
            User? currentUser = await _userService.GetUserByIdAsync(studentId);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            Progresses = await _progressService.GetStudentProgressesDtoForStudentAsync(pageNumber, pageSize, currentUser);
            return Page();
        }
    }
}
