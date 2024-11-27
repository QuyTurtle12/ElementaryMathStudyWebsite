using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.PurchasePages
{
    public class TransactionResultModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVnPayService _vnPayService;
        private readonly IAppOrderServices _orderService;

        public TransactionResultModel(IUnitOfWork unitOfWork, IVnPayService vnPayService, IAppOrderServices orderService)
        {
            _unitOfWork = unitOfWork;
            _vnPayService = vnPayService;
            _orderService = orderService;
        }

        public VnPayResponseDto vnPayResponse = default!;
        public Order userCart = default!;
        public async Task<IActionResult> OnGetAsync()
        {
            string? sessionPurchase = HttpContext.Session.GetString("session_purchase");

            if (string.IsNullOrWhiteSpace(sessionPurchase))
            {
                return Content("");
            }
            vnPayResponse = _vnPayService.PaymentExecute(Request.Query);
            userCart = (await _unitOfWork.GetRepository<Order>().GetByIdAsync(vnPayResponse.OrderId))!;
            string response;

            if (vnPayResponse.VnPayResponseCode != "00")
            {
                response = await _orderService.HandleVnPayCallback(vnPayResponse.OrderId, false);
				ViewData["Title"] = "Transaction failed";
			}
			else
            {
                response = await _orderService.HandleVnPayCallback(vnPayResponse.OrderId, true);
				ViewData["Title"] = "Transaction successful";
			}

			HttpContext.Session.Remove("session_purchase");

            return Page();

        }
    }
}
