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

        public async Task<IActionResult> OnGetAsync()
        {
            string? userId = HttpContext.Session.GetString("user_id");

            if (userId == null) 
            {
                return Content("");
            }

            string questionId = "15A8C741-E3FF-42BE-89BD-865DC9006113";
            Question question = (await _unitOfWork.GetRepository<Question>().GetByIdAsync(questionId))!;

            if (string.IsNullOrWhiteSpace(questionId))
            {
                return Content("");
            }

            options = await _optionService.GetOptionDtosByQuestion(-1, -1, questionId);

            return Page();
        }
    }
}
