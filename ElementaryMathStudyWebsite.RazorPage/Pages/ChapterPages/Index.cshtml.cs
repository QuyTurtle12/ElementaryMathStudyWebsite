using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ChapterPages
{
    public class IndexModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;
        private const int PageSize = 10;

        public IndexModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public List<ChapterViewDto> Chapters { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string? SubjectId { get; set; }

        public async Task<IActionResult> OnGetAsync(string? handler)
        {
            var query = _context.Chapter
                .Include(c => c.Quiz)
                .Include(c => c.Subject)
                .Where(c => string.IsNullOrEmpty(c.DeletedBy));

            // Thêm điều kiện lọc theo SubjectId nếu có
            if (!string.IsNullOrEmpty(SubjectId))
            {
                query = query.Where(c => c.SubjectId == SubjectId);
                var subject = await _context.Subject.FindAsync(SubjectId);
                if (subject != null)
                {
                    ViewData["SubjectName"] = subject.SubjectName;
                }
            }

            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                query = query.Where(c => EF.Functions.Like(c.ChapterName, $"%{SearchKeyword}%"));
            }

            // Đếm tổng số items
            TotalItems = await query.CountAsync();

            // Tính tổng số trang
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Điều chỉnh số trang nếu cần
            CurrentPage = PaginationHelper.ValidateAndAdjustPageNumber(PageNumber, TotalItems, PageSize);

            // Lấy dữ liệu theo trang
            Chapters = await query
                .OrderBy(c => c.Number)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ChapterViewDto
                {
                    Id = c.Id,
                    Number = c.Number,
                    ChapterName = c.ChapterName,
                    Status = c.Status,
                    SubjectId = c.SubjectId,
                    SubjectName = c.Subject!.SubjectName,
                    QuizId = c.QuizId,
                    QuizName = c.Quiz!.QuizName
                })
                .ToListAsync();

            // Xử lý request AJAX
            if (!string.IsNullOrEmpty(handler))
            {
                if (handler == "search")
                {
                    return Partial("_ChapterTableRows", this);
                }
                else if (handler == "pagination")
                {
                    return Partial("_Pagination", this);
                }
            }

            return Page();
        }
    }
}