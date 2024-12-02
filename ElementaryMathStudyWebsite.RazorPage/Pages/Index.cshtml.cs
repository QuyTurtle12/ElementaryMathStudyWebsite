using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Services.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElementaryMathStudyWebsite.RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IAppSubjectServices _subjectService;

        public List<SubjectDTO> AvailableSubjects { get; private set; } = new();

        [BindProperty]
        public string SelectedSubjectId { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IAppSubjectServices appSubjectServices)
        {
            _logger = logger;
            _subjectService = appSubjectServices;
        }

        public async Task<IActionResult> OnGet()
        {
            // Fetch available subjects
            AvailableSubjects = (await _subjectService.GetAllSubjectsAsync(-1, -1, false))
                .Items
                .OfType<SubjectDTO>()
                .ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (SelectedSubjectId == string.Empty)
            {
                ModelState.AddModelError("", "Please select a valid subject.");
                return Page();
            }

            // Redirect to Chapter Index with the selected subject ID (GUID)
            return RedirectToPage("/ChapterPages/IndexPartial", new { subjectId = SelectedSubjectId });
        }

        public JsonResult OnGetSessionInfo()
        {
            var userId = HttpContext.Session.GetString("user_id");
            var roleId = HttpContext.Session.GetString("role_id");

            // Return session info as JSON
            return new JsonResult(new { userId, roleId });
        }

        public IActionResult OnPostLogout()
        {
            // Clear session data
            HttpContext.Session.Clear();
            return RedirectToPage("AuthPages/Login"); // Redirect to login page
        }
    }
}
