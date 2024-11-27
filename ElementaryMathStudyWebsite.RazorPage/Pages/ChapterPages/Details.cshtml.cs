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
    public class DetailsModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public DetailsModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public Chapter Chapter { get; set; } = default!;
        public string SubjectName { get; set; } = default!;
        public string QuizName { get; set; } = default!;
        public string CreatedByName { get; set; } = default!;
        public string LastUpdatedByName { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Chapter = await _context.Chapter
                .Include(c => c.Subject)
                .Include(c => c.Quiz)
                .FirstOrDefaultAsync(m => m.Id.ToString() == id);

            if (Chapter == null)
            {
                return NotFound();
            }

            SubjectName = Chapter.Subject?.SubjectName ?? "";
            QuizName = Chapter.Quiz?.QuizName ?? "";

            var createdByUser = await _context.User.FindAsync(Chapter.CreatedBy);
            CreatedByName = createdByUser?.FullName ?? "";

            var lastUpdatedByUser = await _context.User.FindAsync(Chapter.LastUpdatedBy);
            LastUpdatedByName = lastUpdatedByUser?.FullName ?? "";

            return Page();
        }
    }
}
