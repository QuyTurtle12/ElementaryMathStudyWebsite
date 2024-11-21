using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.OrderPages
{
	[Authorize(Policy = "Parent")]
	public class IndexModel : PageModel
    {
        private readonly IAppOrderServices _orderService;
        private readonly IAppUserServices _userService;

		public IndexModel(IAppOrderServices orderServices, IAppUserServices userService)
		{
			_orderService = orderServices;
			_userService = userService;
		}

		public BasePaginatedList<OrderViewDto>? Orders { get; set; } = default!;


		public async Task<IActionResult> OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
            string currentUserId = HttpContext.Session.GetString("user_id")!;
            User? currentUser = await _userService.GetUserByIdAsync(currentUserId);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            Orders = await _orderService.GetOrderDtosAsync(pageNumber, pageSize, currentUser);
            return Page();
        }
    }
}
