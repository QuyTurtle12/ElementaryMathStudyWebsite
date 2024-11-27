using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.Quiz
{
    public class CreateModel : PageModel
    {
        private readonly IAppQuizServices _quizService;
        private readonly IAppUserServices _userService;

        public CreateModel(IAppQuizServices quizService, IAppUserServices userService)
        {
            _quizService = quizService;
            _userService = userService;
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

                QuizMainViewDto? result = await _quizService.AddQuizAsync(quizCreateDto, currentUser);
                if (result == null)
                {
                    TempData["ErrorMessage"] = "Quiz name already exists.";
                    return Page();
                }
                else
                {
                    TempData["SuccessMessage"] = "Quiz created successfully!";
                    return Page();
                }    
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return Page();
            }
        }


    }
}
