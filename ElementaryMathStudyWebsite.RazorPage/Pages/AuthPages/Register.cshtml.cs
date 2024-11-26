using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using System.ComponentModel.DataAnnotations;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.AuthPages
{

    public class RegisterModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        public RegisterModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context, IUnitOfWork unitOfWork,
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
            // Map User entity to RegisterDto for validation
            var registerDto = new RegisterDto
            {
                Username = User.Username,
                Password = User.Password,
                Email = User.Email ?? "",
                FullName = User.FullName,
                PhoneNumber = User.PhoneNumber,
                Gender = User.Gender
            };

            // Check if the user already exists
            var existingUser = await _unitOfWork.GetRepository<User>().FindByConditionAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email);
            if (existingUser != null && existingUser.DeletedBy == null)
            {
                ModelState.AddModelError(string.Empty, "Username or email already exists.");
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

            var role = await _unitOfWork.GetRepository<Role>().FindByConditionAsync(r => r.RoleName == "Parent");
            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid role name.");
                return Page();
            }
            User.Role = role;
            User.Password = BCrypt.Net.BCrypt.HashPassword(User.Password);
            User.Status = false;
            User.VerificationToken = Guid.NewGuid().ToString();
            
            _context.User.Add(User);
            await _context.SaveChangesAsync();

            // Send verification email
            await _emailService.SendVerificationEmailAsyncV2(User.Email ?? "", User.VerificationToken);

            return RedirectToPage("/AuthPages/Login");
        }
    }
}
