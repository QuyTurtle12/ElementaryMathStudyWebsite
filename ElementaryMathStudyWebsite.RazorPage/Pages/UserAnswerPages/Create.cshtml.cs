using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserAnswerPages
{
    [Authorize(Policy = "Student")]
    public class CreateModel : PageModel
    {
        private readonly DatabaseContext _context;

        public CreateModel(DatabaseContext context)
        {
            _context = context;
        }

        public string CurrentUserId { get; private set; } = string.Empty;

        public List<QuestionViewModel> Questions { get; set; } = new();
        public List<OptionViewModel> Options { get; set; } = new();

        public IActionResult OnGet()
        {
            // Retrieve the current user ID
            CurrentUserId = HttpContext.Session.GetString("user_id");

            // Preload all questions
            Questions = _context.Question
                .Select(q => new QuestionViewModel
                {
                    Id = q.Id,
                    QuestionContext = q.QuestionContext
                })
                .ToList();

            // Preload all options
            Options = _context.Option
                .Select(o => new OptionViewModel
                {
                    Id = o.Id,
                    QuestionId = o.QuestionId,
                    Answer = o.Answer
                })
                .ToList();

            return Page();
        }

        [BindProperty]
        public UserAnswer UserAnswer { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns in case of validation failure
                Questions = _context.Question
                    .Select(q => new QuestionViewModel
                    {
                        Id = q.Id,
                        QuestionContext = q.QuestionContext
                    })
                    .ToList();

                Options = _context.Option
                    .Select(o => new OptionViewModel
                    {
                        Id = o.Id,
                        QuestionId = o.QuestionId,
                        Answer = o.Answer
                    })
                    .ToList();

                if (!Options.Any())
                {
                    Console.Write("error option");
                }

                return Page();
            }

            // Ensure UserId is set to the current user
            UserAnswer.UserId = HttpContext.Session.GetString("user_id");

            // Add new UserAnswer entry
            _context.UserAnswer.Add(UserAnswer);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public class QuestionViewModel
        {
            public string Id { get; set; }
            public string QuestionContext { get; set; }
        }

        public class OptionViewModel
        {
            public string Id { get; set; }
            public string QuestionId { get; set; }
            public string Answer { get; set; }
        }
    }
}
