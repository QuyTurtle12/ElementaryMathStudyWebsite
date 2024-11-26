using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.QuestionPages
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
        public Question question = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            string questionId = "15A8C741-E3FF-42BE-89BD-865DC9006113";
            question = (await _unitOfWork.GetRepository<Question>().GetByIdAsync(questionId))!;

            if (string.IsNullOrWhiteSpace(questionId))
            {
                return Content("");
            }

            options = await _optionService.GetOptionDtosByQuestion(-1, -1, questionId);

            return Page();
        }
    }
}
