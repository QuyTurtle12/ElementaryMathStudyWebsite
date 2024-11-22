using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.AuthPages
{

    public class RegisterModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public RegisterModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public IActionResult OnGet()
        {
        ViewData["RoleId"] = new SelectList(_context.Role, "Id", "RoleName");
            return Page();
        }

        [BindProperty]
        public new User User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var role = await _unitOfWork.GetRepository<Role>().FindByConditionAsync(r => r.RoleName == "Parent");
            if (role == null)
            {
                throw new BaseException.CoreException("invalid_argument", "Invalid role name.");
            }
            User.Role = role;
            User.Password = BCrypt.Net.BCrypt.HashPassword(User.Password);
            User.Status = false;

            _context.User.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
