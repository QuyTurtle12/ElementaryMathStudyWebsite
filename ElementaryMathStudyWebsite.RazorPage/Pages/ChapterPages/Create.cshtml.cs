using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ChapterPages
{
    public class CreateModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public CreateModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["QuizId"] = new SelectList(_context.Quiz, "Id", "QuizName");
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "SubjectName");
            return Page();
        }

        [BindProperty]
        public Chapter Chapter { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Kiểm tra xem tên chương đã tồn tại hay chưa
            var existingChapterName = await _context.Chapter
                .FirstOrDefaultAsync(c => c.ChapterName == Chapter.ChapterName);

            if (existingChapterName != null)
            {
                ModelState.AddModelError(string.Empty, "Tên chương này đã tồn tại.");
                ViewData["QuizId"] = new SelectList(_context.Quiz, "Id", "QuizName");
                ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "SubjectName");
                return Page();
            }

            // Kiểm tra xem QuizName đã được gắn cho một Chapter hoặc Topic nào chưa
            var existingChapter = await _context.Chapter
                .FirstOrDefaultAsync(c => c.QuizId == Chapter.QuizId);
            var existingTopic = await _context.Topic
                .FirstOrDefaultAsync(t => t.QuizId == Chapter.QuizId);

            if (existingChapter != null || existingTopic != null)
            {
                ModelState.AddModelError(string.Empty, "Quiz này đã được gắn cho một Chapter hoặc Topic khác.");
                ViewData["QuizId"] = new SelectList(_context.Quiz, "Id", "QuizName");
                ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "SubjectName");
                return Page();
            }

            // Tự động tăng số thứ tự và kiểm tra trùng lặp
            int number = await GetNextAvailableNumberAsync(Chapter.SubjectId);
            Chapter.Number = number;

            AuditFields(Chapter, isCreating: true); 

            _context.Chapter.Add(Chapter);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task<int> GetNextAvailableNumberAsync(string subjectId)
        {
            int number = await _context.Chapter
                .Where(c => c.SubjectId == subjectId)
                .MaxAsync(c => (int?)c.Number) ?? 0;

            bool numberExists;
            do
            {
                number++;
                numberExists = await _context.Chapter
                    .AnyAsync(c => c.SubjectId == subjectId && c.Number == number);
            } while (numberExists);

            return number;
        }

        private void AuditFields(BaseEntity entity, bool isCreating = false)
        {
            string currentUserId = HttpContext.Session.GetString("user_id") ?? "";

            if (isCreating)
            {
                entity.CreatedBy = currentUserId; 
                entity.CreatedTime = DateTime.UtcNow;
            }

            entity.LastUpdatedBy = currentUserId; 
            entity.LastUpdatedTime = DateTime.UtcNow;
        }
    }
}
