using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.TopicPages
{
    public class IndexModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public IndexModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public List<TopicViewDto> Topic { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; private set; }
        public int CurrentPage { get; private set; }

        private const int PageSize = 10;

        public async Task<IActionResult> OnGetAsync()
        {
            IQueryable<Topic> query = _context.Topic
                .Include(u => u.Chapter);

            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                query = query.Where(u => EF.Functions.Like(u.TopicName, $"%{SearchKeyword}%"));
            }

            // Count total items for pagination
            int totalItems = await query.CountAsync();

            // Calculate total pages
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            CurrentPage = PageNumber;

            // Fetch paginated results
            Topic = await query
               .Skip((PageNumber - 1) * PageSize)
               .Take(PageSize)
               .Select(u => new TopicViewDto
               {
                   Id = u.Id,
                   Number = u.Number,
                   QuizId = u.QuizId,
                   TopicName = u.TopicName,
                   ChapterId = u.ChapterId,
                   ChapterName = u.Chapter!.ChapterName
               })
               .ToListAsync();

            return Page();
        }
    }
}
