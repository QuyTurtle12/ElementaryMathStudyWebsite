using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserAnswerPages
{
    [Authorize(Policy = "Student")]
    public class CreateModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public CreateModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserAnswer UserAnswer { get; set; } = default!;

        public IActionResult OnGet(string? questionId = null)
        {
            // Get the current user ID from the session
            string userId = HttpContext.Session.GetString("user_id") ?? "";

            // Retrieve the user's full name
            string? userName = _context.User.FirstOrDefault(u => u.Id == userId)?.FullName;

            // Store the user's name in ViewData for display
            ViewData["CurrentUserName"] = userName ?? "Unknown User";

            // Populate the question dropdown
            var questions = _context.Question.ToList();
            ViewData["QuestionContext"] = new SelectList(questions, "Id", "QuestionContext", questionId);

            // Ensure questionId is set to the current or default value
            questionId ??= questions.FirstOrDefault()?.Id;

            // Filter options for the specific question
            ViewData["OptionId"] = new SelectList(
                _context.Option
                    .Where(o => o.QuestionId == questionId)
                    .Select(o => new { Id = o.Id, Display = o.Answer }),
                "Id",
                "Display");

            // Set the selected question in the model
            UserAnswer = new UserAnswer {
                UserId = userId,
                QuestionId = questionId 
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? questionId = null)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns to maintain state
                var questions = _context.Question.ToList();
                ViewData["QuestionContext"] = new SelectList(questions, "Id", "QuestionContext", questionId);
                questionId ??= questions.FirstOrDefault()?.Id;

                ViewData["OptionId"] = new SelectList(
                    _context.Option
                        .Where(o => o.QuestionId == questionId)
                        .Select(o => new { Id = o.Id, Display = o.Answer }),
                    "Id",
                    "Display");
                ViewData["UserFullName"] = new SelectList(_context.User, "Id", "FullName");

                return Page();
            }

            // Add the new UserAnswer entry
            _context.UserAnswer.Add(UserAnswer);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}