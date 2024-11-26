using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.UOW;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.QuestionPages
{
    public class DetailsModel : PageModel
    {
        private readonly IAppQuestionServices _questionServices;

        public QuestionMainViewDto Question { get; set; } = default!;

        public DetailsModel(IAppQuestionServices questionServices)
        {
            _questionServices = questionServices;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                // Sử dụng dịch vụ để lấy câu hỏi theo ID
                QuestionMainViewDto questionDto = await _questionServices.GetQuestionByIdAsync(id);
                Question = questionDto;
                return Page();
            }
            catch (BaseException.NotFoundException ex)
            {
                TempData["NotFoundMessage"] = $"Question with '{id}' does not exist";
                return Page();
            }
        }
    }
}
