using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Services.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserAnswerPages
{
    public class IndexModel : PageModel
    {
        private readonly IAppUserAnswerServices _userAnswerService;

        public IndexModel(IAppUserAnswerServices userAnswerService)
        {
            _userAnswerService = userAnswerService;
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

            // Fetch all items using the service
            var allItems = await _userAnswerService.GetAllUserAnswersAsync(-1, -1);

            // Apply search filter if a keyword is provided
            IEnumerable<UserAnswerWithDetailsDTO> filteredItems = allItems.Items
                .Where(f => f.UserId == UserId)
                .OrderBy(f => f.AttemptNumber);

            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                filteredItems = filteredItems.Where(u =>
                    u.QuestionContent != null &&
                    u.QuestionContent.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            // Count total items after filtering
            int totalFilteredItems = filteredItems.Count();

            // Paginate the filtered items
            UserAnswers = filteredItems
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Update pagination details
            TotalPages = (int)Math.Ceiling(totalFilteredItems / (double)PageSize);
            CurrentPage = PageNumber;

            return Page();
        }
    }
}
