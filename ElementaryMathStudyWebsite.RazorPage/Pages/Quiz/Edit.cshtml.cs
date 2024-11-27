using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.Quiz
{
    public class EditModel : PageModel
    {
        private readonly IAppQuizServices _quizService;
        private readonly IAppUserServices _userService;
        public EditModel(IAppQuizServices quizService, IAppUserServices userService)
        {
            _quizService = quizService;
            _userService = userService;
        }

        [BindProperty]
        public QuizMainViewDto Quiz { get; set; } = default!;

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

                string currentUserId = HttpContext.Session.GetString("user_id")!;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["ErrorMessage"] = "User is not authenticated.";
                    return Page();
                }

                User? currentUser = await _userService.GetUserByIdAsync(currentUserId);
                if (currentUser == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return Page();
                }

                QuizMainViewDto? result = await _quizService.UpdateQuizAsync(id, quizUpdateDto, currentUser);
                if (result == null)
                {
                    TempData["ErrorMessage"] = "Quiz name already exists.";
                    return Page();
                }
                else
                {
                    TempData["SuccessMessage"] = "Quiz updated successfully!";
                    return Page();
                }
            }
            catch (BaseException.ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Page();
            }
        }
    }
}
