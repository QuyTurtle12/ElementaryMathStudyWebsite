using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.Quiz
{
    public class CreateModel : PageModel
    {
        private readonly IAppQuizServices _quizService;

        public CreateModel(IAppQuizServices quizService)
        {
            _quizService = quizService;
        }

        [BindProperty]
        public QuizCreateDto Quiz { get; set; } = new QuizCreateDto();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (Quiz == null)
                {
                    TempData["ErrorMessage"] = "Quiz data is missing.";
                    return Page();
                }

                QuizCreateDto quizCreateDto = new QuizCreateDto
                {
                    QuizName = Quiz.QuizName,
                    Criteria = Quiz.Criteria
                };

                QuizMainViewDto result = await _quizService.AddQuizAsync(quizCreateDto);

                TempData["SuccessMessage"] = "Quiz created successfully!";
                return RedirectToPage("./Index");
            }
            catch (BaseException.ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Page();
            }
        }

    }
}
