using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Services.Service;
using Microsoft.AspNetCore.Authorization;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserAnswerPages
{
    [Authorize(Policy = "Student")]
    public class CreateModel : PageModel
    {
        private readonly IAppUserServices _userService;
        private readonly IAppQuestionServices _questionService;
        private readonly IAppOptionServices _optionService;
        private readonly IAppUserAnswerServices _userAnswerService;

        public CreateModel(
        IAppUserServices userService,
            IAppQuestionServices questionService,
            IAppUserAnswerServices userAnswerService,
            IAppOptionServices optionService)
        {
            _userService = userService;
            _questionService = questionService;
            _userAnswerService = userAnswerService;
            _optionService = optionService;
        }

        [BindProperty]
        public UserAnswerCreateDTO UserAnswerDTO { get; set; } = new();

        public string CurrentUserName { get; private set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string? SelectedQuestionId { get; set; }

        public async Task<IActionResult> OnGetAsync(string? questionId = null)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            CurrentUserName = currentUser.FullName;
            SelectedQuestionId = questionId;

            var questions = await _questionService.GetAllQuestionsMainViewDtoAsync();
            ViewData["QuestionContext"] = new SelectList(questions, "Id", "QuestionContent", questionId);

            var options = await _optionService.GetOptionDtosByQuestion(-1, -1, questionId ?? questions.FirstOrDefault()?.Id);
            ViewData["OptionId"] = new SelectList((System.Collections.IEnumerable)options, "Id", "Answer");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                var questions = await _questionService.GetAllQuestionsMainViewDtoAsync();
                ViewData["QuestionContext"] = new SelectList(questions, "Id", "QuestionContent", SelectedQuestionId);

                var options = await _optionService.GetOptionDtosByQuestion(-1, -1, SelectedQuestionId);
                ViewData["OptionId"] = new SelectList((System.Collections.IEnumerable) options, "Id", "Answer");

                return Page();
            }

            // Create the user answer
            await _userAnswerService.CreateUserAnswersAsync(UserAnswerDTO);

            return RedirectToPage("./Index");
        }
    }
}
