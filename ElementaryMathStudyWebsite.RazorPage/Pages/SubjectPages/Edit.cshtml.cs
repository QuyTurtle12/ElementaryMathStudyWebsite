using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Services.Service;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.SubjectPages
{
    public class EditModel : PageModel
    {
        private readonly IAppSubjectServices _appSubjectService;

        public EditModel(IAppSubjectServices subjectService)
        {
            _appSubjectService = subjectService;
        }

        [BindProperty]
        public SubjectUpdateDTO Subject { get; set; } = new SubjectUpdateDTO();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _appSubjectService.GetSubjectByIDAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            Subject = new SubjectUpdateDTO
            {
                SubjectName = subject.SubjectName,
                Price = subject.Price,
                Status = subject.Status
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _appSubjectService.UpdateSubjectAsync(id, Subject);
            }
            catch (Exception ex)
            {
                var existSubject = await _appSubjectService.SearchSubjectExactAsync(Subject.SubjectName);
                if (existSubject != null)
                {
                    ModelState.AddModelError(string.Empty, "Subject name existed!");
                }

                if (Subject.Price <= 0)
                {
                    ModelState.AddModelError("Subject.Price", "Price must be a positive number.");
                    return Page();
                }
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
