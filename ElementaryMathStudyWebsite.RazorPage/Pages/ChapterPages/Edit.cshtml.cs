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
    public class EditModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public EditModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Chapter Chapter { get; set; } = default!;
        public string LastUpdatedByName { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapter.FirstOrDefaultAsync(m => m.Id == id);
            if (chapter == null)
            {
                return NotFound();
            }
            Chapter = chapter;

            var lastUpdatedByUser = await _context.User.FindAsync(Chapter.LastUpdatedBy);
            LastUpdatedByName = lastUpdatedByUser?.FullName ?? "Unknown";

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Kiểm tra xem tên chương đã tồn tại hay chưa
            var existingChapterName = await _context.Chapter
                .FirstOrDefaultAsync(c => c.ChapterName == Chapter.ChapterName && c.Id != Chapter.Id);

            if (existingChapterName != null)
            {
                ModelState.AddModelError(string.Empty, "Tên chương này đã tồn tại.");
                return Page();
            }

            // Lấy thông tin người dùng hiện tại từ session
            string currentUserId = HttpContext.Session.GetString("user_id") ?? "";

            var chapterToUpdate = await _context.Chapter.FirstOrDefaultAsync(c => c.Id == Chapter.Id);
            if (chapterToUpdate != null)
            {
                chapterToUpdate.ChapterName = Chapter.ChapterName;
                chapterToUpdate.LastUpdatedBy = currentUserId;  // Cập nhật ID người dùng hiện tại
                chapterToUpdate.LastUpdatedTime = DateTime.UtcNow;  // Cập nhật thời gian

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChapterExists(Chapter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ChapterExists(string id)
        {
            return _context.Chapter.Any(e => e.Id == id);
        }
    }
}
