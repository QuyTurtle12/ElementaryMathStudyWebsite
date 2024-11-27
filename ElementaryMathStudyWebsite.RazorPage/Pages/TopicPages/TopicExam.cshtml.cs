using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.CodeAnalysis.Options;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.TopicPages
{
    public class TopicExamModel : PageModel
    {
        private readonly DatabaseContext _context;

        public TopicExamModel(DatabaseContext context)
        {
            _context = context;
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
            var subjectId = await _context.Chapter
                .Where(c => c.Id == chapterId)
                .Select(c => c.SubjectId)
                .FirstOrDefaultAsync();

            // Check if the user has access to this subject
            var hasAccess = await _context.OrderDetail
                .AnyAsync(od => od.StudentId == userId && od.SubjectId == subjectId);

            if (!hasAccess)
            {
                TempData["AlertMessage"] = "You do not have access to this topic or exam.";
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

            var questionIds = Questions.Select(q => q.Id).ToList();

            // Load user answers for the current quiz into memory
            var userAnswerAttemp = await _context.UserAnswer
                .Where(ua => ua.UserId == userId && questionIds.Contains(ua.QuestionId))
                .ToListAsync();

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
                    var option = await _context.Option
                        .Where(o => o.Id == selectedOptionId.ToString())
                        .FirstOrDefaultAsync();

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

            // Save user answers
            _context.UserAnswer.AddRange(userAnswers);
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

            return RedirectToPage("./TopicExamResult", new { score = formattedScore });
        }
    }
}
