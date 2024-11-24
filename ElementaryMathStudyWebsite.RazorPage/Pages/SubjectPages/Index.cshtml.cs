using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.ObjectModel;
using System.Linq;

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

        public IList<Subject> UserSubjects { get; set; } = default!; // For SubjectDTO
        public IList<SubjectAdminViewDTO> AdminSubjects { get; set; } = default!; // For SubjectAdminViewDTO
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

            if (IsValidRole)
            {
                var paginatedSubjects = await _appSubjectServices.GetAllSubjectsAsync(-1, -1, true);
                var list = paginatedSubjects.Items.ToList();

                AdminSubjects = list
                    .OfType<SubjectAdminViewDTO>()
                    .Select(dto => new SubjectAdminViewDTO
                    {
                        Id = dto.Id,
                        SubjectName = dto.SubjectName,
                        Price = dto.Price,
                        Status = dto.Status,
                        CreatedBy = dto.CreatedBy,
                        LastUpdatedBy = dto.LastUpdatedBy,
                        CreatedTime = dto.CreatedTime,
                        LastUpdatedTime = dto.LastUpdatedTime
                    })
                    .ToList();
            }
            else
            {
                var paginatedSubjects = await _appSubjectServices.GetAllSubjectsAsync(-1, -1, false);
                var list = paginatedSubjects.Items.ToList();

                UserSubjects = list
                    .OfType<SubjectDTO>()
                    .Select(dto => new Subject
                    {
                        Id = dto.Id,
                        SubjectName = dto.SubjectName,
                        Price = dto.Price,
                        Status = dto.Status
                    })
                    .ToList();
            }

            return Page();
        }
    }
}