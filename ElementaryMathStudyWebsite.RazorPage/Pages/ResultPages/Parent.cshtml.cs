using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ResultPages
{
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

        public async Task<IActionResult> OnGetAsync(string studentId, string chapterOrTopicId, int pageNumber = 1, int pageSize = 5)
        {
            User? currentUser = await _userService.GetUserByIdAsync(studentId);

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
