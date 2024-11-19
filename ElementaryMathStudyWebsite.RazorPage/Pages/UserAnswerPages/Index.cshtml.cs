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

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve session information
            UserId = HttpContext.Session.GetString("user_id");
            UserRole = HttpContext.Session.GetString("role_name");

            IsLoggedIn = true;

            IQueryable<UserAnswer> query = _context.UserAnswer
                .Include(u => u.Question) // Include related Question entity
                .Include(u => u.Option)   // Include related Option entity
                .Include(u => u.User);    // Include related User entity

            // Fetch all user answers and map to DTO
            UserAnswers = await query
                .Select(u => new UserAnswerWithDetailsDTO
                {
                    QuestionId = u.QuestionId,
                    QuestionContent = u.Question.QuestionContext,  // Map QuestionContent
                    UserId = u.UserId,
                    UserFullName = u.User.FullName,       // Map UserFullName
                    OptionId = u.OptionId,
                    OptionAnswer = u.Option.Answer,      // Map OptionAnswer
                    AttemptNumber = u.AttemptNumber
                })
                .ToListAsync();

            return Page();
        }
    }
}
