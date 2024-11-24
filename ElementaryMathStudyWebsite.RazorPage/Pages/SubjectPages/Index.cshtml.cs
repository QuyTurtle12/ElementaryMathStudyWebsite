using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.SubjectPages
{
    public class IndexModel : PageModel
    {
        private readonly IAppSubjectServices _appSubjectServices;
        private readonly IAppUserServices _userService;
        private readonly IMapper _mapper;

        public IndexModel(IAppSubjectServices appSubjectServices, IMapper mapper, IAppUserServices userService)
        {
            _appSubjectServices = appSubjectServices;
            _userService = userService;
            _mapper = mapper;
        }

        public IList<SubjectDTO> UserSubjects { get; set; } = default!;
        public IList<SubjectAdminViewDTO> AdminSubjects { get; set; } = default!;
        public string UserRole { get; private set; } = string.Empty;
        public bool IsLoggedIn { get; private set; } = false;
        public bool IsValidRole { get; private set; } = false;

        [BindProperty(SupportsGet = true)]
        public string SearchName { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public double? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public double? MaxPrice { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetString("user_id");
            UserRole = HttpContext.Session.GetString("role_name");
            IsLoggedIn = !string.IsNullOrEmpty(userId);

            if (!IsLoggedIn)
            {
                return Redirect("/");
            }

            // Validate role
            IsValidRole = UserRole == "Admin" || UserRole == "Content Manager" || UserRole == "Manager";

            // Check if the request contains search parameters
            bool isSearchRequest = !string.IsNullOrEmpty(SearchName) || MinPrice.HasValue || MaxPrice.HasValue;

            if (!isSearchRequest || string.IsNullOrEmpty(SearchName))
            {
                // If not a search request, load default data (e.g., all subjects or paginated data)
                if (IsValidRole)
                {
                    var paginatedSubjects = await _appSubjectServices.GetAllSubjectsAsync(-1, -1, true);
                    AdminSubjects = paginatedSubjects.Items.OfType<SubjectAdminViewDTO>().ToList();
                }
                else
                {
                    var paginatedSubjects = await _appSubjectServices.GetAllSubjectsAsync(-1, -1, false);
                    UserSubjects = paginatedSubjects.Items.OfType<SubjectDTO>().ToList();
                }

                return Page();
            }

            // Apply search filters if this is a search request
            if (!MinPrice.HasValue) MinPrice = -1;
            if (!MaxPrice.HasValue) MaxPrice = -1;

            if (IsValidRole)
            {
                var paginatedSubjects = await _appSubjectServices.SearchSubjectAdminAsync(SearchName, (double)MinPrice, (double)MaxPrice, null, -1, -1);
                AdminSubjects = paginatedSubjects.Items.OfType<SubjectAdminViewDTO>().ToList();
            }
            else
            {
                var paginatedSubjects = await _appSubjectServices.SearchSubjectAsync(SearchName, (double)MinPrice, (double)MaxPrice, -1, -1);
                UserSubjects = paginatedSubjects.Items.OfType<SubjectDTO>().ToList();
            }

            return Page();
        }
    }
}
