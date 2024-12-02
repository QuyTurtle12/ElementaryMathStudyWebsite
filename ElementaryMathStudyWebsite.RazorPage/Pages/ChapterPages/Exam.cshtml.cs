using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ChapterPages
{
    public class ExamModel : PageModel
    {
        private readonly IAppOrderDetailServices _orderDetailService;
        private readonly IAppQuestionServices _questionService;
        private readonly IAppOptionServices _optionService;
        private readonly IAppUserAnswerServices _userAnswerService;

        public ExamModel(IAppOrderDetailServices orderDetailService, IAppQuestionServices appQuestionServices, IAppOptionServices optionService, IAppUserAnswerServices appUserAnswerServices)
        {
            _orderDetailService = orderDetailService;
            _questionService = appQuestionServices;
            _optionService = optionService;
            _userAnswerService = appUserAnswerServices;
        }

        [BindProperty]
        public string QuizId { get; set; }

        public IList<Question> Questions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string quizId, string subjectId)
        {
            var userId = HttpContext.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["AlertMessage"] = "You must be logged in to access the exam.";
                return Page();
            }

            // Check if the user has access to this subject
            //var hasAccess = await _context.OrderDetail
            //    .AnyAsync(od => od.StudentId == userId && od.SubjectId == subjectId);
            var hasAccess = await _orderDetailService.IsOrderDetailExistsAsync(userId, subjectId);

            if (!hasAccess)
            {
                TempData["AlertMessage"] = "You do not have access to this chapter or exam.";
                return Page();
            }

            QuizId = quizId;

            // Fetch questions and options for the quiz
            Questions = await _questionService.GetQuestionsWithOptionsEntitiesByQuizIdAsync(quizId);

            if (Questions == null || !Questions.Any())
            {
                TempData["AlertMessage"] = "No questions found for the specified quiz.";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to take the exam.");
                return Page();
            }

            // Fetch questions again since Questions is not persisted between requests
            Questions = await _questionService.GetQuestionsWithOptionsEntitiesByQuizIdAsync(QuizId);

            if (Questions == null || !Questions.Any())
            {
                ModelState.AddModelError(string.Empty, "No questions found for the specified quiz.");
                return Page();
            }

            // Materialize Question IDs into a list
            var questionIds = Questions.Select(q => q.Id).ToList();

            // Load user answers for the current quiz into memory
            //var userAnswers = await _context.UserAnswer
            //    .Where(ua => ua.UserId == userId && questionIds.Contains(ua.QuestionId))
            //    .ToListAsync();
            var userAnswers = await _userAnswerService.GetUserAnswersByUserAndQuestionsAsync(userId, questionIds);

            // Calculate the max attempt number for this user and quiz
            var maxAttemptNumber = userAnswers.Any()
                ? userAnswers.Max(ua => ua.AttemptNumber)
                : 0;

            var answers = new List<UserAnswer>();
            var correctAnswersCount = 0;

            foreach (var question in Questions)
            {
                var selectedOptionValue = Request.Form[$"Answer_{question.Id}"];

                // Convert the value to a string
                var selectedOptionId = selectedOptionValue.FirstOrDefault();
                if (string.IsNullOrEmpty(selectedOptionId)) // Check if an answer was provided
                {
                    ModelState.AddModelError(string.Empty, $"Please answer question {Questions.IndexOf(question) + 1}.");
                    return Page(); // Reload the page with the error message
                }

                // Query the database using the string ID
                var option = await _optionService.GetOptionByIdAsync(selectedOptionId.ToString());

                if (option != null)
                {
                    answers.Add(new UserAnswer
                    {
                        UserId = userId,
                        QuestionId = question.Id,
                        OptionId = option.Id,
                        AttemptNumber = maxAttemptNumber + 1 // Increment the attempt number
                    });

                    if (option.IsCorrect)
                    {
                        correctAnswersCount++;
                    }
                }
            }

            // Create a DTO for saving user answers
            var userAnswerCreateDTO = new UserAnswerCreateDTO
            {
                UserAnswerList = userAnswers.Select(ua => new UserAnswersDTO
                {
                    QuestionId = ua.QuestionId,
                    OptionId = ua.OptionId
                }).ToList()
            };

            // Save user answers
            var result = await _userAnswerService.CreateUserAnswersUserAsync(userAnswerCreateDTO, userId);

            // Calculate and format the score
            var totalQuestions = Questions.Count;
            var totalPoints = (correctAnswersCount / (float)totalQuestions) * 10;
            var formattedScore = $"{correctAnswersCount}/{totalQuestions}";

            // Store the formatted score and points as strings in TempData
            TempData["Score"] = formattedScore;
            TempData["Points"] = totalPoints.ToString("F1"); // Format as string with 1 decimal place

            return RedirectToPage("./ExamResult", new { score = formattedScore });
        }
    }
}
