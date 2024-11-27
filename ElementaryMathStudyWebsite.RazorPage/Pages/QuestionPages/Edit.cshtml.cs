using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.QuestionPages
{
    public class EditModel : PageModel
    {
        private readonly IAppQuestionServices _questionService;
        private readonly IAppUserServices _userService;

        public EditModel(IAppQuestionServices questionService, IAppUserServices userService)
        {
            _questionService = questionService;
            _userService = userService;
        }

        [BindProperty]
        public QuestionMainViewDto Question { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the Question details from the service by ID
            Question = await _questionService.GetQuestionByIdAsync(id);

            // If no question is found, return NotFound
            if (Question == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                QuestionUpdateDto questionUpdateDto = new QuestionUpdateDto
                {
                    QuestionContext = Question.QuestionContext,
                    QuizId = Question.QuizId
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

                QuestionMainViewDto? result = await _questionService.UpdateQuestionAsync(id, questionUpdateDto, currentUser);
                if (result == null)
                {
                    TempData["ErrorMessage"] = "Question name already exists.";
                    return Page();
                }
                else
                {
                    TempData["SuccessMessage"] = "Question updated successfully!";
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
