using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.OrderPages
{
    public class DetailsModel : PageModel
    {
        private readonly IAppOrderServices _orderService;
        public IEnumerable<OrderDetailViewDto> orderDetails;
        public DetailsModel(IAppOrderServices orderService)
        {
            _orderService = orderService;
        }

        public OrderAdminViewDto Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Order = await _orderService.GetOrderAdminDtoASync(id);
            orderDetails = Order.Details ?? new List<OrderDetailViewDto>();
            return Page();
        }
    }
}
