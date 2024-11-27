using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
namespace ElementaryMathStudyWebsite.RazorPage.Pages.QuestionPages
{
    public class DeleteModel : PageModel
    {
        private readonly IAppQuestionServices _questionService;

        public DeleteModel(IAppQuestionServices questionService)
        {
            _questionService = questionService;
        }

        [BindProperty]
        public  QuestionMainViewDto Question { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the Question details from the service by ID
            Question = await _questionService.GetQuestionByIdAsync(id);

            // If no question is found, return NotFound
            if (Question == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Attempt to delete the question
            var response = await _questionService.DeleteQuestionAsync(id);


            // Show message
            if (!string.IsNullOrEmpty(response.Data))
            {
                TempData["SuccessMessage"] = response.Data;
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to delete the question. Please try again.";
            }

            return RedirectToPage("./Index");
        }
    }
}
