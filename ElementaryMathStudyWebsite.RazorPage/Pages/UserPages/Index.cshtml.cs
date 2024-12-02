using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Authorization;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Services.Service;
using AutoMapper;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserPages
{
    [Authorize(Policy = "Admin-Manager")]
    public class IndexModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;
        private readonly IAppUserServices _userService;
        private readonly IMapper _mapper;
        public IndexModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context, IAppUserServices userService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }

        public new IList<User> User { get; set; } = default!;

        public BasePaginatedList<UserResponseDto>? Users { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
            var paginatedUsers = await _userService.GetAllUsersAsync(pageNumber, pageSize);
            // Map users to UserResponseDto
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(paginatedUsers.Items);

            // Create a new paginated list of UserResponseDto
            Users = new BasePaginatedList<UserResponseDto>(
                userDtos.ToList(),
                paginatedUsers.TotalItems,
                paginatedUsers.CurrentPage,
                paginatedUsers.PageSize
            );
            return Page();

        }
    }
}
