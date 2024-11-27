using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.OptionPages
{
    public class CreateModel : PageModel
    {
        private readonly IAppOptionServices _optionService;

        public CreateModel(IAppOptionServices optionService)
        {
            _optionService = optionService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public OptionCreateDto Option { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            string userId = HttpContext.Session.GetString("user_id")!;

            string questionId = "15A8C741-E3FF-42BE-89BD-865DC9006113";
            Option.QuestionId = questionId;

            await _optionService.AddOption(userId, Option);

            return RedirectToPage("./Index");
        }
    }
}
