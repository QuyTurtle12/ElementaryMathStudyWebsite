using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> OnGetAsync(string id)
        {
            QuizId = id;

            // Fetch questions and options for the quiz using GUID
            Questions = await _context.Question
                .Where(q => q.QuizId == id)
                .Include(q => q.Options)
                .ToListAsync();

            if (Questions == null || !Questions.Any())
            {
                return NotFound("No questions found for the specified quiz.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userId))
            {
                // Redirect to an Unauthorized page or show an error message
                ModelState.AddModelError(string.Empty, "You must be logged in to take the exam.");
                return Page();
            }

            var answers = new List<UserAnswer>();
            var correctAnswersCount = 0;

            foreach (var question in Questions)
            {
                var selectedOptionId = Request.Form[$"Answer_{question.Id}"];
                if (Guid.TryParse(selectedOptionId, out Guid optionId))
                {
                    var option = await _context.Option.FindAsync(optionId);
                    if (option != null)
                    {
                        answers.Add(new UserAnswer
                        {
                            UserId = userId,
                            QuestionId = question.Id,
                            OptionId = option.Id
                        });

                        if (option.IsCorrect)
                        {
                            correctAnswersCount++;
                        }
                    }
                }
            }

            // Save all user answers
            _context.UserAnswer.AddRange(answers);
            await _context.SaveChangesAsync();

            // Calculate score
            var totalQuestions = Questions.Count;
            var totalPoints = (correctAnswersCount / (float)totalQuestions) * 10;

            TempData["Score"] = totalPoints;
            return RedirectToPage("./ExamResult", new { score = totalPoints });
        }
    }
}
