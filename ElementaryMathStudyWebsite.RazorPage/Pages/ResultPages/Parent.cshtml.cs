using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ResultPages
{
    [Authorize(Policy = "Parent")]
    public class ParentModel : PageModel
    {
        private readonly IAppUserServices _userService;
        private readonly IAppResultService _resultService;

        public ParentModel(IAppUserServices userService, IAppResultService resultService)
        {
            _userService = userService;
            _resultService = resultService;
        }

        public BasePaginatedList<ResultViewDto> Results { get; set; } = default!;
        static string localChapterOrTopicId { get; set; } = string.Empty;
        static string localstudentId { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string studentId, string chapterOrTopicId, int pageNumber = 1, int pageSize = 5)
        {
            if (!string.IsNullOrWhiteSpace(studentId)) localstudentId = studentId;

            User? currentStudent = await _userService.GetUserByIdAsync(localstudentId);

            if (currentStudent == null)
            {
                return RedirectToPage("/AccessDenied");
            }

            if (!string.IsNullOrWhiteSpace(chapterOrTopicId)) localChapterOrTopicId = chapterOrTopicId;

            string quizId = await _resultService.GetQuizIdByChapterOrTopicId(localChapterOrTopicId);

            Results = await _resultService.GetStudentResultListAsync(currentStudent, quizId, pageNumber, pageSize);

            return Page();
        }
    }
}
