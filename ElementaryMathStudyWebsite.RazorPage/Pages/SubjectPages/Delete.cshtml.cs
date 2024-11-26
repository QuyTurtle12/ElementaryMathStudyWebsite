using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Services.Service;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.SubjectPages
{
    public class DeleteModel : PageModel
    {
        private readonly IAppSubjectServices _appSubjectService;

        public DeleteModel(IAppSubjectServices subjectService)
        {
            _appSubjectService = subjectService;
        }

        [BindProperty]
        public Subject Subject { get; set; } = default!;
        public string? SubjectName { get; set; }
        public double? Price { get; set; }
        public string? CreatedByName { get; set; }
        public string? LastUpdatedByName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }

        public IActionResult OnGet(string id, string? subjectName, double? price, string? createdByName, string? lastUpdatedByName,
                                   string? createdTime, string? lastUpdatedTime)
        {
            if (id == null)
            {
                return NotFound();
            }

            SubjectName = subjectName ?? "No name";
            Price = price ?? 0.0;

            // Assign values from route parameters to properties
            CreatedByName = createdByName ?? "Unknown";
            LastUpdatedByName = lastUpdatedByName ?? "Unknown";

            CreatedTime = DateTime.TryParse(createdTime, out var parsedCreatedTime) ? parsedCreatedTime : null;
            LastUpdatedTime = DateTime.TryParse(lastUpdatedTime, out var parsedLastUpdatedTime) ? parsedLastUpdatedTime : null;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // Use the soft delete service
                await _appSubjectService.SoftDeleteSubjectAsync(id);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., not found or validation errors)
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}