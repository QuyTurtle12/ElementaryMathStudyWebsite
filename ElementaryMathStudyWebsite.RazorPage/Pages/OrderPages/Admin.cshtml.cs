using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.OrderPages
{
    [Authorize(Policy = "Admin")]
    public class AdminModel : PageModel
    {
        private readonly IAppOrderServices _orderService;
        private readonly IAppUserServices _userService;

        public AdminModel(IAppOrderServices orderService, IAppUserServices userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        public BasePaginatedList<OrderAdminViewDto> Orders { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
            string currentUserId = HttpContext.Session.GetString("user_id")!;
            User? currentUser = await _userService.GetUserByIdAsync(currentUserId);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            Orders = await _orderService.GetOrderAdminDtosAsync(pageNumber, pageSize);
            return Page();
        }
    }
}
