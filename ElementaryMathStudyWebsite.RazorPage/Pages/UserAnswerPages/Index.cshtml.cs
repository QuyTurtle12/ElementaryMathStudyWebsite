using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserAnswerPages
{
    public class IndexModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public IndexModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public List<UserAnswerWithDetailsDTO> UserAnswers { get; set; } = new();
        public string UserRole { get; private set; } = string.Empty;
        public bool IsLoggedIn { get; private set; } = false;
        public string UserId { get; private set; } = string.Empty;
        public bool IsValidRole { get; private set; } = false;

        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; private set; }
        public int CurrentPage { get; private set; }

        private const int PageSize = 10;

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve session information
            UserId = HttpContext.Session.GetString("user_id");
            UserRole = HttpContext.Session.GetString("role_name");
            IsLoggedIn = !string.IsNullOrEmpty(UserId);

            // Determine if the user role is valid
            IsValidRole = UserRole == "Student" || UserRole == "Admin" || UserRole == "Manager" || UserRole == "Content Manager";

            if (!IsValidRole)
            {
                return Page(); // Show the alert message
            }

            // Base query
            IQueryable<UserAnswer> query = _context.UserAnswer
                .Include(u => u.Question)
                .Include(u => u.Option)
                .Include(u => u.User);

            // If the user is a student, filter answers by their UserId
            if (UserRole == "Student")
            {
                query = query.Where(u => u.UserId == UserId);
            }

            // Apply search filter if keyword is provided
            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                query = query.Where(u => EF.Functions.Like(u.Question.QuestionContext, $"%{SearchKeyword}%"));
            }

            // Count total items for pagination
            int totalItems = await query.CountAsync();

            // Calculate total pages
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            CurrentPage = PageNumber;

            // Fetch paginated results
            UserAnswers = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(u => new UserAnswerWithDetailsDTO
                {
                    QuestionId = u.QuestionId,
                    QuestionContent = u.Question.QuestionContext,
                    UserId = u.UserId,
                    UserFullName = u.User.FullName,
                    OptionId = u.OptionId,
                    OptionAnswer = u.Option.Answer,
                    AttemptNumber = u.AttemptNumber
                })
                .ToListAsync();

            return Page();
        }
    }
}
