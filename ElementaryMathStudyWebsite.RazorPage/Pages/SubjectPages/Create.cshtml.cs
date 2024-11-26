using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Services.Service;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.SubjectPages
{
    public class CreateModel : PageModel
    {
        private readonly IAppSubjectServices _appSubjectService;
        private readonly IAppUserServices _userService;

        public CreateModel(IAppSubjectServices appSubjectService, IAppUserServices userService)
        {
            _appSubjectService = appSubjectService;
            _userService = userService;
        }

        [BindProperty]
        public SubjectCreateDTO Subject { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Create a new subject
                var createdSubject = await _appSubjectService.CreateSubjectAsync(new SubjectDTO
                {
                    Id = string.Empty, // Let the service handle ID generation
                    SubjectName = Subject.SubjectName,
                    Price = Subject.Price,
                    Status = true
                });

                var updateSubject = await _appSubjectService.SearchSubjectExactAsync(createdSubject.SubjectName);
                var userId = HttpContext.Session.GetString("user_id");
                await _appSubjectService.AuditSubjectAsync(updateSubject, userId, true);

                TempData["SuccessMessage"] = "Subject created successfully!";
                return RedirectToPage("./Index");
            }
            catch (BaseException.BadRequestException ex)
            {
                var existSubject = await _appSubjectService.SearchSubjectExactAsync(Subject.SubjectName);
                if (existSubject != null)
                {
                    ModelState.AddModelError(string.Empty, "Subject name existed!");
                }

                if(Subject.Price <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Price must be greater than 0");
                }
            }
            catch (Exception ex)
            {
                // Handle generic errors
                ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
            }

            // Return to the same page with error messages displayed
            return Page();
        }
    }
}
