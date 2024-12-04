using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Authorization;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserPages
{
    [Authorize(Policy = "Admin-Manager")]
    public class EditModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;
        private readonly IAppUserServices _userService;
        private readonly IMapper _mapper;
        public EditModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context, IAppUserServices userService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }

        [BindProperty]
        public new User User { get; set; } = default!;
        public UpdateUserDto Update { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User? user = await _context.User.Include(u => u.Role)
                                            .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;
           ViewData["RoleId"] = new SelectList(_context.Role, "Id", "RoleName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Update = new UpdateUserDto()
            {
                Username = User.Username,
                Email = User.Email,
                FullName = User.FullName,
                Gender = User.Gender,
                PhoneNumber = User.PhoneNumber,
                RoleId = User.RoleId
            };

            try
            {
                await _userService.UpdateUserAsync(User.Id, Update);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
