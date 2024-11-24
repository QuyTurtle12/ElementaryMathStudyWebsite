using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Services.Service;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.Quiz
{
    public class DetailsModel : PageModel
    {
        private readonly IAppQuizServices _quizService;
        private readonly IAppQuestionServices _questionService;

        public DetailsModel(IAppQuizServices quizService, IAppQuestionServices questionService)
        {
            _quizService = quizService;
            _questionService = questionService;
        }

        [BindProperty]
        public QuizMainViewDto Quiz { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 5;

        public QuizViewDto Quizdto { get; set; }
        public BasePaginatedList<QuestionViewDto> PaginatedQuestions { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the quiz details
            Quiz = await _quizService.GetQuizByQuizIdAsync(id);

            // Fetch paginated questions for the quiz
            PaginatedQuestions = await _questionService.GetQuestionsByQuizIdAsync(id, PageNumber, PageSize);

            // If no quiz or questions are found, return NotFound
            if (Quiz == null || PaginatedQuestions == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
