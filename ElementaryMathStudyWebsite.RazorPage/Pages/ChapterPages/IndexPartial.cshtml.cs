using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ChapterPages
{
    public class IndexPartialModel : PageModel
    {
        private readonly DatabaseContext _context;
        private const int PageSize = 10;

        public IndexPartialModel(DatabaseContext context)
        {
            _context = context;
        }

        public List<ChapterViewDto> Chapters { get; set; } = new();
        public string SubjectName { get; set; } = string.Empty;
        public int CurrentPage { get; set; }
         public string SubjectId { get; set; } = string.Empty; 
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync(string subjectId, string? handler)
        {
            if (string.IsNullOrEmpty(subjectId))
            {
                return NotFound();
            }

            var subject = await _context.Subject.FindAsync(subjectId);
            if (subject == null)
            {
                return NotFound();
            }

            SubjectName = subject.SubjectName;

            var query = _context.Chapter
                .Include(c => c.Quiz)
                .Where(c => c.SubjectId == subjectId && string.IsNullOrEmpty(c.DeletedBy));

            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                var searchKeywordLower = SearchKeyword.ToLower();
                query = query.Where(c => 
                    (c.ChapterName != null && c.ChapterName.ToLower().Contains(searchKeywordLower)) ||
                    (c.Quiz != null && c.Quiz.QuizName != null && c.Quiz.QuizName.ToLower().Contains(searchKeywordLower)));
            }

            // Đếm tổng số items
            TotalItems = await query.CountAsync();

            // Tính tổng số trang
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Điều chỉnh số trang
            CurrentPage = PaginationHelper.ValidateAndAdjustPageNumber(PageNumber, TotalItems, PageSize);

            // Lấy dữ liệu theo trang
            var skip = (CurrentPage - 1) * PageSize;
            Chapters = await query
                .OrderBy(c => c.Number)
                .Skip(skip)
                .Take(PageSize)
                .Select(c => new ChapterViewDto
                {
                    Id = c.Id,
                    Number = c.Number,
                    ChapterName = c.ChapterName ?? string.Empty,
                    QuizId = c.QuizId,
                    QuizName = c.Quiz != null ? c.Quiz.QuizName ?? string.Empty : string.Empty,
                    SubjectId = c.SubjectId
                })
                .ToListAsync();

            if (!string.IsNullOrEmpty(handler))
            {
                var tableHtml = "";
                var paginationHtml = "";

                // Render table rows
                foreach (var item in Chapters)
                {
                    tableHtml += $@"<tr>
                        <td>{item.Number}</td>
                        <td>{item.ChapterName}</td>
                        <td>{item.QuizName}</td>
                        <td>
                            <a href='./Exam?quizId={item.QuizId}&subjectId={item.SubjectId}' 
                            class='btn btn-primary btn-sm'>
                                <i class='fas fa-book'></i> Take Exam
                            </a>
                        </td>
                    </tr>";
                }

                // Render pagination
                paginationHtml = $@"<div class='d-flex justify-content-between align-items-center mt-4'>
                    <div>
                        Tổng số: {TotalItems} chương
                    </div>
                    <div class='d-flex gap-3 align-items-center'>";

                if (CurrentPage > 1)
                {
                    paginationHtml += $@"<button onclick='changePage({CurrentPage - 1})' 
                            class='btn btn-secondary'>
                        <i class='fas fa-chevron-left'></i> Trang trước
                    </button>";
                }

                paginationHtml += $@"<span>
                        Trang {CurrentPage} / {TotalPages}
                    </span>";

                if (CurrentPage < TotalPages)
                {
                    paginationHtml += $@"<button onclick='changePage({CurrentPage + 1})' 
                            class='btn btn-secondary'>
                        Trang sau <i class='fas fa-chevron-right'></i>
                    </button>";
                }

                paginationHtml += "</div></div>";

                return new JsonResult(new { tableHtml, paginationHtml });
            }

            return Page();
        }
    }
}
