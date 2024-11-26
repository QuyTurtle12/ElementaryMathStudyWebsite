using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq.Expressions;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.AuthPages
{
    public class VerifyEmailModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public VerifyEmailModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Token))
            {
                Message = "Invalid token.";
                return Page();
            }

            Expression<Func<User, bool>> condition = u => u.VerificationToken == Token;

            var user = await _unitOfWork.GetRepository<User>().FindByConditionWithIncludesAsync(
                condition,
                u => u.Role! // Include the Role if needed
                // Add other includes here if needed
            );

            if (user == null)
            {
                Message = "Invalid token";
                return Page();
            }
            if (!user.Role!.RoleName.Equals("Student"))
            {
                user.CreatedBy = user.Id;
            }
            // Verify the user
            user.VerificationToken = null;
            user.Status = true; // Mark as verified
            await _context.SaveChangesAsync();

            Message = "Your email has been verified successfully!";
            return Page();
        }
    }
}
