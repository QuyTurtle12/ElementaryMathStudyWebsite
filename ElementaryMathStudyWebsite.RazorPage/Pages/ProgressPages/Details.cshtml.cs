using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ProgressPages
{
	public class DetailsModel : PageModel
    {
		private IAppProgressServices _progressService;

        public string StudentId { get; set; } = string.Empty;

        public DetailsModel(IAppProgressServices progressService)
        {
            _progressService = progressService;
        }

        public IEnumerable<FinishedTopic>? FinishedTopics { get; set; } = new List<FinishedTopic>();

		public IEnumerable<FinishedChapter>? FinishedChapters { get; set; } = new List<FinishedChapter>();

        static string localstudentId { get; set; } = string.Empty;
        static string localsubjectId { get; set; } = string.Empty;

        public IActionResult OnGet(string studentId, string subjectId)
		{
            // Prevent session delete student Id and subject Id when move to next page
            if (!string.IsNullOrWhiteSpace(studentId)) localstudentId = studentId;
            if (!string.IsNullOrWhiteSpace(subjectId)) localsubjectId = subjectId;

            // Receive student Id from the previous page
            StudentId = localstudentId;

            (IEnumerable<FinishedTopic> finishedTopics, IEnumerable<FinishedChapter> finishedChapters) = _progressService.GetFinishedTopicsAndChaptersModified(localstudentId, localsubjectId);

            // Assign the passed collections to the properties
            FinishedTopics = finishedTopics;
			FinishedChapters = finishedChapters;

			return Page();
		}
	}
}
