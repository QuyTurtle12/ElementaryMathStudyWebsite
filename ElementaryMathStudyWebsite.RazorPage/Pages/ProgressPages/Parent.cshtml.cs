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
        static string localstudentId { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string studentId, int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(studentId)) localstudentId = studentId;

                User? currentUser = await _userService.GetUserByIdAsync(localstudentId);

                if (currentUser == null)
                {
                    return Unauthorized();
                }

                Progresses = await _progressService.GetStudentProgressesDtoForStudentAsync(pageNumber, pageSize, currentUser);
                
            }
            catch (BaseException.NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.ErrorDetail.ErrorMessage;
                TempData["ErrorCode"] = ex.ErrorDetail.ErrorCode;
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                TempData["ErrorCode"] = "internal_error";
                return Page();
            }

            return Page();
        }
    }
}
