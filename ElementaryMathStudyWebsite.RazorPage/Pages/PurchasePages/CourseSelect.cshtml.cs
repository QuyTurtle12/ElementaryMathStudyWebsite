using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.CheckoutPages
{
	public class CourseSelectModel : PageModel
	{
		private readonly IAppSubjectServices _subjectService;
		private readonly IAppOrderServices _orderService;
		private readonly IUnitOfWork _unitOfWork;

		public CourseSelectModel(IAppSubjectServices subjectService, IUnitOfWork unitOfWork, IAppOrderServices orderService)
		{
			_subjectService = subjectService;
			_unitOfWork = unitOfWork;
			_orderService = orderService;

		}

		public BasePaginatedList<object> subjects = default!;  // Property should be public
		public OrderViewDto addItemsObject = default!;

		[BindProperty]
		public string? SelectedSubjectId { get; set; }  // This will hold the selected subject's ID from the form
		public string? studentId;
		public async Task<IActionResult> OnGetAsync()
		{
			studentId = HttpContext.Session.GetString("selected_student_id");

			if (string.IsNullOrWhiteSpace(studentId))
			{
				return Content("");
			}

			subjects = await _subjectService.GetAllSubjectsAsync(-1, -1, false);

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{

			try
			{
				studentId = HttpContext.Session.GetString("selected_student_id");

				if (string.IsNullOrWhiteSpace(SelectedSubjectId))
				{
					TempData["ErrorMessage"] = "No subject selected.";
					return Page();  // You can return an error message or stay on the page
				}

				HttpContext.Session.SetString("selected_subject_id", SelectedSubjectId);


				CartCreateDto cartCreateDto = new()
				{
					StudentId = studentId!,
					SubjectId = SelectedSubjectId
				};

				string userId = HttpContext.Session.GetString("user_id")!;

				addItemsObject = await _orderService.AddItemsToCart(userId, cartCreateDto);



				return Redirect("/PurchasePages/Cart");
			}
			catch (Exception)
			{
				TempData["ErrorMessage"] = "Học sinh đã sở hữu khóa học này";  // Set a generic error message

				return RedirectToPage();
			}



		}

	}
}
