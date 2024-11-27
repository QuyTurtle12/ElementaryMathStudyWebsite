using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.AuthPages
{
    public class RegistChildModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        public RegistChildModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context, IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
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
            string id = HttpContext.Session.GetString("user_id") ?? "";

            if (string.IsNullOrWhiteSpace(id)) // Check if user_id is set in the session
            {
                return RedirectToPage("/AuthPages/LoginError"); // Redirect to login if session is missing
            }
            var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return RedirectToPage("/AuthPages/LoginError");
            }

            // Map User entity to RegisterDto for validation
            var registerDto = new StudentRegisterDto
            {
                Username = User.Username,
                Password = User.Password,
                FullName = User.FullName,
                PhoneNumber = User.PhoneNumber,
                Gender = User.Gender
            };

            // Check if the user already exists
            var existingUser = await _unitOfWork.GetRepository<User>().FindByConditionAsync(u => u.Username == registerDto.Username);
            if (existingUser != null && existingUser.DeletedBy == null)
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                return Page();
            }

            // Validate the DTO
            var validationContext = new ValidationContext(registerDto);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(registerDto, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    ModelState.AddModelError("", validationResult.ErrorMessage ?? "Error");
                }
                return Page();
            }

            var role = await _unitOfWork.GetRepository<Role>().FindByConditionAsync(r => r.RoleName == "Student");
            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid role.");
                return Page();
            }
            User.Role = role;
            User.Password = BCrypt.Net.BCrypt.HashPassword(User.Password);
            User.Status = false;
            User.CreatedBy = id;
            User.VerificationToken = Guid.NewGuid().ToString();

            _context.User.Add(User);
            await _context.SaveChangesAsync();

            // Send verification email
            await _emailService.SendVerificationEmailAsyncV2(user.Email ?? "", User.VerificationToken);

            return RedirectToPage("/UserPages/ParentInfo");
        }
    }
}
