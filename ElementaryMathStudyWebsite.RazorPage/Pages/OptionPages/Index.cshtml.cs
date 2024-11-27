using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.OptionPages
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppOptionServices _optionService;

        public IndexModel(IUnitOfWork unitOfWork, IAppOptionServices optionService)
        {
            _unitOfWork = unitOfWork;
            _optionService = optionService;
        }

        public BasePaginatedList<OptionViewDto> options = default!;  // Property should be public

        public async Task<IActionResult> OnGetAsync(string id)
        {
            string? userId = HttpContext.Session.GetString("user_id");

            if (userId == null) 
            {
                return Content("");
            }

            //Question question = (await _unitOfWork.GetRepository<Question>().GetByIdAsync(id))!;

            if (string.IsNullOrWhiteSpace(id))
            {
                return Content("");
            }

            options = await _optionService.GetOptionDtosByQuestion(-1, -1, id);

            return Page();
        }
    }
}
