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
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ChapterPages
{
    [Authorize(Policy = "Admin-Content")]
    public class IndexModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;
        private const int PageSize = 10;

        public IndexModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public List<ChapterViewDto> Chapters { get; set; } = new();
        public List<SubjectDTO> Subjects { get; set; } = new();
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
                var searchKeywordLower = SearchKeyword.ToLower();
                query = query.Where(c => 
                    (c.ChapterName != null && c.ChapterName.ToLower().Contains(searchKeywordLower)) ||
                    (c.Subject != null && c.Subject.SubjectName != null && c.Subject.SubjectName.ToLower().Contains(searchKeywordLower)) ||
                    (c.Quiz != null && c.Quiz.QuizName != null && c.Quiz.QuizName.ToLower().Contains(searchKeywordLower)));
            }
            // Thêm đoạn code lấy danh sách subjects vào đầu method
            Subjects = await _context.Subject
                .Where(s => string.IsNullOrEmpty(s.DeletedBy))
                .Select(s => new SubjectDTO 
                { 
                    Id = s.Id, 
                    SubjectName = s.SubjectName,
                    Price = s.Price,
                    Status = s.Status
                })
                .ToListAsync();

            // Đếm tổng số items
            TotalItems = await query.CountAsync();

            // Tính tổng số trang
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Điều chỉnh số trang nếu cần
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
                    Status = c.Status,
                    SubjectId = c.SubjectId,
                    SubjectName = c.Subject != null ? c.Subject.SubjectName ?? string.Empty : string.Empty,
                    QuizId = c.QuizId,
                    QuizName = c.Quiz != null ? c.Quiz.QuizName ?? string.Empty : string.Empty
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
                        <td>{item.SubjectName}</td>
                        <td>{item.QuizName}</td>
                        <td>
                            <div class='d-flex gap-2'>
                                <a href='/ChapterPages/Details?id={item.Id}' 
                                class='btn btn-info btn-sm'>
                                    <i class='fas fa-info-circle'></i> Details
                                </a>
                                <a href='/ChapterPages/Edit?id={item.Id}' 
                                class='btn btn-warning btn-sm'>
                                    <i class='fas fa-edit'></i> Edit
                                </a>
                                <a href='/ChapterPages/Delete?id={item.Id}' 
                                class='btn btn-danger btn-sm'>
                                    <i class='fas fa-trash'></i> Delete
                                </a>
                            </div>
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