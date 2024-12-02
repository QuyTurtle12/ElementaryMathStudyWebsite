using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.TopicPages
{
    public class TopicExamModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppChapterServices _chapterService;
        private readonly IAppOrderDetailServices _orderDetailService;
        private readonly IAppQuestionServices _questionService;
        private readonly IAppOptionServices _optionService;
        private readonly IAppUserAnswerServices _userAnswerService;

        public TopicExamModel(IAppChapterServices appChapterServices, IUnitOfWork unitOfWork, IAppOrderDetailServices orderDetailService, IAppQuestionServices appQuestionServices, IAppOptionServices optionService, IAppUserAnswerServices appUserAnswerServices)
        {
            _chapterService = appChapterServices;
            _unitOfWork = unitOfWork;
            _orderDetailService = orderDetailService;
            _questionService = appQuestionServices;
            _optionService = optionService;
            _userAnswerService = appUserAnswerServices;
        }

        [BindProperty]
        public string QuizId { get; set; }

        public IList<Question> Questions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string quizId, string chapterId)
        {
            Console.WriteLine(chapterId);
            var userId = HttpContext.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["AlertMessage"] = "You must be logged in to access the exam.";
                return Page();
            }

            // Retrieve the SubjectId associated with the ChapterId
            ChapterAdminViewDto getChapter = (ChapterAdminViewDto) await _chapterService.GetChapterByChapterIdAsync(chapterId);
            var subjectId = getChapter.SubjectId;

            // Check if the user has access to this subject
            var hasAccess = await _orderDetailService.IsOrderDetailExistsAsync(userId, subjectId);

            if (!hasAccess)
            {
                TempData["AlertMessage"] = "You do not have access to this topic or exam.";
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

            var questionIds = Questions.Select(q => q.Id).ToList();

            // Load user answers for the current quiz into memory
            //var userAnswerAttemp = await _context.UserAnswer
            //    .Where(ua => ua.UserId == userId && questionIds.Contains(ua.QuestionId))
            //    .ToListAsync();
            var userAnswerAttemp = await _userAnswerService.GetUserAnswersByUserAndQuestionsAsync(userId, questionIds);

            // Calculate the max attempt number for this user and quiz
            var maxAttemptNumber = userAnswerAttemp.Any()
                ? userAnswerAttemp.Max(ua => ua.AttemptNumber)
                : 0;

            // Process user answers and save to UserAnswer table
            var userAnswers = new List<UserAnswer>();
            var correctAnswersCount = 0;
            foreach (var question in Questions)
            {
                var selectedOptionId = Request.Form[$"Answer_{question.Id}"];
                if (!string.IsNullOrEmpty(selectedOptionId))
                {
                    //var option = await _context.Option
                    //    .Where(o => o.Id == selectedOptionId.ToString())
                    //    .FirstOrDefaultAsync();
                    var option = await _optionService.GetOptionByIdAsync(selectedOptionId.ToString());

                    userAnswers.Add(new UserAnswer
                    {
                        UserId = userId,
                        QuestionId = question.Id,
                        OptionId = selectedOptionId,
                        AttemptNumber = maxAttemptNumber + 1
                    });

                    if (option.IsCorrect)
                    {
                        correctAnswersCount++;
                    }
                }
            }

            if (userAnswers.Count != Questions.Count)
            {
                ModelState.AddModelError(string.Empty, "You must answer all questions.");
                return Page();
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

            return RedirectToPage("./TopicExamResult", new { score = formattedScore });
        }
    }
}
