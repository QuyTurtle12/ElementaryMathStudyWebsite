using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.TopicPages
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseContext _context;

        public IndexModel(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IList<Topic> Topic { get; set; } = default!;
        public string? SearchString { get; set; }
        public string? SelectedChapterId { get; set; } // Thêm thuộc tính cho chương đã chọn
        public SelectList Chapters { get; set; } = default!; // Danh sách chương

        // Phân trang
        public int TotalPages { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public async Task OnGetAsync(string? searchString, string? selectedChapterId, int pageIndex = 1)
        {
            PageIndex = pageIndex;
            SearchString = searchString;
            SelectedChapterId = selectedChapterId;

            // Lấy danh sách chương từ cơ sở dữ liệu
            var chaptersList = await _context.Chapter.ToListAsync();
            Chapters = new SelectList(chaptersList, "Id", "Name", SelectedChapterId);

            // Khởi tạo truy vấn cho topics
            var topicsQuery = _context.Topic.Include(t => t.Chapter).AsQueryable();

            // Nếu có searchString thì lọc theo tên chủ đề
            if (!string.IsNullOrEmpty(SearchString))
            {
                topicsQuery = topicsQuery.Where(s => s.TopicName.Contains(SearchString));
            }

            // Nếu không có selectedChapterId, hiển thị tất cả các chủ đề
            // Nếu có selectedChapterId, lọc theo ChapterId
            if (!string.IsNullOrEmpty(SelectedChapterId))
            {
                topicsQuery = topicsQuery.Where(t => t.ChapterId == SelectedChapterId);
            }

            // Đếm số lượng chủ đề
            var count = await topicsQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            // Lấy danh sách chủ đề theo phân trang
            Topic = await topicsQuery.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();
        }
    }
}