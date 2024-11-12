using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.OrderPages
{
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

		public int PageNumber { get; set; } = 1; // default page number
		public int PageSize { get; set; } = 10; // default page size

		public async Task<IActionResult> OnGetAsync()
        {
            string currentUserId = HttpContext.Session.GetString("user_id")!;
            User? currentUser = await _userService.GetUserByIdAsync(currentUserId);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            Orders = await _orderService.GetOrderDtosAsync(PageNumber, PageSize, currentUser);
            return Page();
        }
    }
}
