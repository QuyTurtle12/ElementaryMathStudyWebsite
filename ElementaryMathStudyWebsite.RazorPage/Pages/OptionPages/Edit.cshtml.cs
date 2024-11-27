using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.OptionPages
{
    public class EditModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly IAppOptionServices _optionService;
        private readonly IUnitOfWork _unitOfWork;

        public EditModel(DatabaseContext context, IAppOptionServices optionService, IUnitOfWork unitOfWork)
        {
            _context = context;
            _optionService = optionService;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Option Option { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Option = (await _unitOfWork.GetRepository<Option>().GetByIdAsync(id))!;
            if (id is null || Option is null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string userId = HttpContext.Session.GetString("user_id")!;

            OptionUpdateDto optionUpdateDto = new()
            {
                Id = Option.Id,
                Answer = Option.Answer,
                IsCorrect = Option.IsCorrect
            };
            Console.WriteLine(optionUpdateDto.Id);
            await _optionService.UpdateOption(userId, optionUpdateDto);

            return RedirectToPage("./Index");
        }
    }
}
