using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ProgressPages
{
	public class DetailsModel : PageModel
    {
		private IAppProgressServices _progressService;

        public DetailsModel(IAppProgressServices progressService)
        {
            _progressService = progressService;
        }

        public IEnumerable<FinishedTopic>? FinishedTopics { get; set; } = new List<FinishedTopic>();

		public IEnumerable<FinishedChapter>? FinishedChapters { get; set; } = new List<FinishedChapter>();

		public IActionResult OnGet(string studentId, string subjectId)
		{
            (IEnumerable<FinishedTopic> finishedTopics, IEnumerable<FinishedChapter> finishedChapters) = _progressService.GetFinishedTopicsAndChaptersModified(studentId, subjectId);

            // Assign the passed collections to the properties
            FinishedTopics = finishedTopics;
			FinishedChapters = finishedChapters;

			return Page();
		}
	}
}
