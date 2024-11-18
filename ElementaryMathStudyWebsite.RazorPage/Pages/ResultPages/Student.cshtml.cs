using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Mvc;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ResultPages
{
    public class StudentModel : PageModel
    {
        private readonly IAppUserServices _userService;
        private readonly IAppResultService _resultService;

        public StudentModel(IAppUserServices userService, IAppResultService resultService)
        {
            _userService = userService;
            _resultService = resultService;
        }

        public BasePaginatedList<ResultViewDto> Results { get;set; } = default!;

        static string localChapterOrTopicId { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string chapterOrTopicId, int pageNumber = 1, int pageSize = 5)
        {
            string currentUserId = HttpContext.Session.GetString("user_id")!;
            User? currentUser = await _userService.GetUserByIdAsync(currentUserId);

            if (currentUser == null)
            {
                return RedirectToPage("/AccessDenied");
            }

            if (!string.IsNullOrWhiteSpace(chapterOrTopicId)) localChapterOrTopicId = chapterOrTopicId;

            string quizId = await _resultService.GetQuizIdByChapterOrTopicId(localChapterOrTopicId);

            Results = await _resultService.GetStudentResultListAsync(currentUser, quizId, pageNumber, pageSize);

            return Page();
        }
    }
}
