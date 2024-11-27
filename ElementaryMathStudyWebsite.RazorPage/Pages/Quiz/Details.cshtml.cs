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

        public BasePaginatedList<QuestionViewDto> PaginatedQuestions { get; set; } = new BasePaginatedList<QuestionViewDto>(new List<QuestionViewDto>(), 0, 1, 5);
        public async Task<IActionResult> OnGetAsync(string id, int pageNumber = 1, int pageSize = 5)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Quiz ID is required.";
                return RedirectToPage("./Index");
            }

            // Fetch the quiz details
            Quiz = await _quizService.GetQuizByQuizIdAsync(id);

            // Fetch paginated questions for the quiz
            PaginatedQuestions = await _questionService.GetQuestionsByQuizIdAsync(id, pageNumber, pageSize);

            // If no questions exist
            if (PaginatedQuestions == null || !PaginatedQuestions.Items.Any())
            {
                TempData["ErrorMessage"] = "This quiz does not contain any questions.";
            }

            return Page();
        }
    }
}
