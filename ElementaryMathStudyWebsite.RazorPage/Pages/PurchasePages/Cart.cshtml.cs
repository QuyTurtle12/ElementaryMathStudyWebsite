using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.PurchasePages
{
    public class CartModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppOrderServices _orderService;
        private readonly IVnPayService _vpnPayService;
        public CartModel(IUnitOfWork unitOfWork, IAppOrderServices orderService, IVnPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _vpnPayService = vnPayService;
        }

        public User student = default!;
        public Subject subject = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
				string? userId = HttpContext.Session.GetString("user_id");
				if (string.IsNullOrWhiteSpace(userId))
				{
					return Content("");
				}

				OrderViewDto cartInfo = await _orderService.ViewCart(userId);
                student = _unitOfWork.GetRepository<User>().GetById(cartInfo.Details!.FirstOrDefault()!.StudentId)!;
                subject = _unitOfWork.GetRepository<Subject>().GetById(cartInfo.Details!.FirstOrDefault()!.SubjectId)!;
			}
			catch (Exception) 
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống";
            }
            
            return Page();
		}

        public async Task<IActionResult> OnPostAsync()
        {
			string? userId = HttpContext.Session.GetString("user_id");
			if (string.IsNullOrWhiteSpace(userId))
			{
				return Content("");
			}
			string url = await _vpnPayService.CreatePaymentUrl(userId, HttpContext);

            HttpContext.Session.SetString("session_purchase", "There is a valid checkpoint for the purchase action");
            return Redirect(url);
        }
    }
}
