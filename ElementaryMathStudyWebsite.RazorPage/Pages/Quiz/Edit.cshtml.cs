using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.Quiz
{
    public class EditModel : PageModel
    {
        private readonly IAppQuizServices _quizService;

        public EditModel(IAppQuizServices quizService)
        {
            _quizService = quizService;
        }

        [BindProperty]
        public QuizMainViewDto Quiz { get; set; } = default!; // Đảm bảo sử dụng QuizMainViewDto

        // GET method to load the quiz data by ID
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the Quiz details from the service by ID
            Quiz = await _quizService.GetQuizByQuizIdAsync(id);

            // If no quiz is found, return NotFound
            if (Quiz == null)
            {
                return NotFound();
            }

            return Page();
        }

        // POST method to update quiz details
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                QuizUpdateDto quizUpdateDto = new QuizUpdateDto
                {
                    QuizName = Quiz.QuizName,
                    Criteria = Quiz.Criteria,
                    Status = Quiz.Status
                };

                Quiz = await _quizService.UpdateQuizAsync(id, quizUpdateDto);

                TempData["SuccessMessage"] = "Quiz updated successfully!";
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
