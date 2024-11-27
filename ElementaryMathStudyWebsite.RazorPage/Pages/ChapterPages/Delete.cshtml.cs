using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using System.Security.Claims;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ChapterPages
{
    public class DeleteModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteModel(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public Chapter Chapter { get; set; } = default!;
        public string SubjectName { get; set; } = default!;
        public string QuizName { get; set; } = default!;
        public string CreatedByName { get; set; } = default!;
        public string LastUpdatedByName { get; set; } = default!;
        public string DeletedByName { get; set; } = default!;

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

            if (!string.IsNullOrEmpty(chapter.DeletedBy))
            {
                var deletedByUser = await _context.User.FindAsync(chapter.DeletedBy);
                DeletedByName = deletedByUser?.FullName ?? "";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            try 
            {
                if (id == null)
                {
                    return NotFound();
                }

                var chapter = await _context.Chapter.FindAsync(id);
                if (chapter == null)
                {
                    return NotFound();
                }

                // Lấy userId từ Claims
                var userId = HttpContext.Session.GetString("user_id");
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToPage("/AuthPages/Login");
                }

                // Thực hiện soft delete
                chapter.DeletedBy = userId;
                chapter.DeletedTime = DateTime.Now;
                chapter.LastUpdatedBy = userId;
                chapter.LastUpdatedTime = DateTime.Now;

                _context.Chapter.Update(chapter);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            catch
            {
                // Log lỗi nếu cần
                return RedirectToPage("./Error");
            }
        }
    }
}
