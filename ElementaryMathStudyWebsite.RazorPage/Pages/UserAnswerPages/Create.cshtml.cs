using System;
using System.Collections.Generic;
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

        public IActionResult OnGet()
        {
        ViewData["OptionId"] = new SelectList(_context.Option, "Id", "Id");
        ViewData["QuestionId"] = new SelectList(_context.Question, "Id", "Id");
        ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public UserAnswer UserAnswer { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.UserAnswer.Add(UserAnswer);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
