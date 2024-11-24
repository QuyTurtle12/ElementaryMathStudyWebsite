using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ChapterPages
{
    public class DeleteModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public DeleteModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Chapter Chapter { get; set; } = default!;
        public string SubjectName { get; set; } = default!;
        public string QuizName { get; set; } = default!;
        public string CreatedByName { get; set; } = default!;
        public string LastUpdatedByName { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapter
                .Include(c => c.Subject)
                .Include(c => c.Quiz)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chapter == null)
            {
                return NotFound();
            }

            Chapter = chapter;
            SubjectName = chapter.Subject?.SubjectName ?? "";
            QuizName = chapter.Quiz?.QuizName ?? "";

            var createdByUser = await _context.User.FindAsync(chapter.CreatedBy);
            CreatedByName = createdByUser?.FullName ?? "";

            var lastUpdatedByUser = await _context.User.FindAsync(chapter.LastUpdatedBy);
            LastUpdatedByName = lastUpdatedByUser?.FullName ?? "";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapter.FindAsync(id);
            if (chapter != null)
            {
                Chapter = chapter;
                _context.Chapter.Remove(Chapter);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
