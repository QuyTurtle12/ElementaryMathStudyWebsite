using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ChapterPages
{
    public class ExamModel : PageModel
    {
        private readonly DatabaseContext _context;

        public ExamModel(DatabaseContext context)
        {
            _context = context;
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
            var hasAccess = await _context.OrderDetail
                .AnyAsync(od => od.StudentId == userId && od.SubjectId == subjectId);

            if (!hasAccess)
            {
                TempData["AlertMessage"] = "You do not have access to this chapter or exam.";
                return Page();
            }

            QuizId = quizId;

            // Fetch questions and options for the quiz
            Questions = await _context.Question
                .Where(q => q.QuizId == quizId)
                .Include(q => q.Options)
                .ToListAsync();

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
            Questions = await _context.Question
                .Where(q => q.QuizId == QuizId)
                .Include(q => q.Options)
                .ToListAsync();

            if (Questions == null || !Questions.Any())
            {
                ModelState.AddModelError(string.Empty, "No questions found for the specified quiz.");
                return Page();
            }

            // Materialize Question IDs into a list
            var questionIds = Questions.Select(q => q.Id).ToList();

            // Load user answers for the current quiz into memory
            var userAnswers = await _context.UserAnswer
                .Where(ua => ua.UserId == userId && questionIds.Contains(ua.QuestionId))
                .ToListAsync();

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
                var option = await _context.Option
                    .Where(o => o.Id == selectedOptionId)
                    .FirstOrDefaultAsync();

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

            // Save all user answers
            _context.UserAnswer.AddRange(answers);
            await _context.SaveChangesAsync();

            // Calculate and format the score
            var totalQuestions = Questions.Count;
            var totalPoints = (correctAnswersCount / (float)totalQuestions) * 10;
            var formattedScore = $"{correctAnswersCount}/{totalQuestions}";

            // Add progress if the score is above 8
            if (totalPoints > 8)
            {
                var subjectId = Request.Query["subjectId"].ToString(); // Retrieve subjectId from query string
                if (Guid.TryParse(subjectId, out Guid parsedSubjectId))
                {
                    var progress = new Progress
                    {
                        StudentId = userId, // Assuming userId can be converted to Guid
                        QuizId = QuizId, // Assuming QuizId is stored as string and convertible to Guid
                        SubjectId = subjectId
                    };

                    _context.Progress.AddRange(progress);
                    await _context.SaveChangesAsync();
                }
            }

                // Store the formatted score and points as strings in TempData
                TempData["Score"] = formattedScore;
                TempData["Points"] = totalPoints.ToString("F1"); // Format as string with 1 decimal place

            return RedirectToPage("./ExamResult", new { score = formattedScore });
        }
    }
}
