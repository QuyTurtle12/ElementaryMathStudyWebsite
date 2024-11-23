using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ChapterPages
{
    public class IndexModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public IndexModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public List<ChapterViewDto> Chapter { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; private set; }
        public int CurrentPage { get; private set; }

        private const int PageSize = 10;

        //public IList<Chapter> Chapter { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            //Chapter = await _context.Chapter
            //    .Include(c => c.Quiz)
            //    .Include(c => c.Subject).ToListAsync();

            IQueryable<Chapter> query = _context.Chapter
                .Include(u => u.Quiz)
                .Include(u => u.Subject);

            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                query = query.Where(u => EF.Functions.Like(u.ChapterName, $"%{SearchKeyword}%"));
            }

            // Count total items for pagination
            int totalItems = await query.CountAsync();

            // Calculate total pages
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            CurrentPage = PageNumber;

            //Fetch paginated results
           Chapter = await query
               .Skip((PageNumber - 1) * PageSize)
               .Take(PageSize)
               .Select(u => new ChapterViewDto
               {
                   Id = u.Id,
                   Number = u.Number,
                   ChapterName = u.ChapterName,
                   Status = u.Status,
                   SubjectId = u.SubjectId,
                   SubjectName = u.Subject!.SubjectName,
                   QuizId = u.QuizId,
                   QuizName = u.Quiz!.QuizName
               })
               .ToListAsync();

            return Page();
        }
    }
}
