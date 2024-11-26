using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.Quiz
{
    public class DeleteModel : PageModel
    {
        private readonly IAppQuizServices _quizService;

        // Constructor
        public DeleteModel(IAppQuizServices quizService)
        {
            _quizService = quizService;
        }

        // Property to bind the Quiz data (using QuizMainViewDto directly)
        [BindProperty]
        public QuizMainViewDto Quiz { get; set; } = default!;

        // GET method to load the Quiz data before deletion
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the Quiz details from the service by ID
            Quiz = await _quizService.GetQuizByQuizIdAsync(id);

            // If no quiz is found, return NotFound
            if (Quiz == null)
            {
                return NotFound();
            }

            return Page();
        }

        // POST method to handle the deletion of the quiz
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                // Attempt to delete the quiz through the service
                var response = await _quizService.DeleteQuizAsync(id);

                // Show success or error message
                if (!string.IsNullOrEmpty(response.Data))
                {
                    TempData["SuccessMessage"] = response.Data;
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to delete the quiz. Please try again.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception and show a user-friendly message
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            // Redirect back to the index page after handling
            return RedirectToPage("./Index");
        }
    }
}
