using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserAnswerPages
{
    public class DeleteModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public DeleteModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserAnswer UserAnswer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var useranswer = await _context.UserAnswer.FirstOrDefaultAsync(m => m.QuestionId == id);

            if (useranswer == null)
            {
                return NotFound();
            }
            else
            {
                UserAnswer = useranswer;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var useranswer = await _context.UserAnswer.FindAsync(id);
            if (useranswer != null)
            {
                UserAnswer = useranswer;
                _context.UserAnswer.Remove(UserAnswer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
