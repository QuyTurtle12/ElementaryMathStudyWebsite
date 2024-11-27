using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.QuestionPages
{
    public class IndexModel : PageModel
    {
        private readonly IAppQuestionServices _questionServices;
        public string CurrentQuestionName = string.Empty;

        public IndexModel(IAppQuestionServices questionServices)
        {
            _questionServices = questionServices;
        }

        public BasePaginatedList<QuestionMainViewDto> QuestionDtos { get; set; } = new BasePaginatedList<QuestionMainViewDto>(new List<QuestionMainViewDto>(), 0, 1, 10);

        public async Task OnGetAsync(string questionName, int pageNumber = 1, int pageSize = 10)
        {
            CurrentQuestionName = questionName;

            if (string.IsNullOrEmpty(questionName))
            {
                QuestionDtos = await _questionServices.GetQuestionsMainViewAsync(pageNumber, pageSize);
            }
            else
            {
                await SearchQuestionsAsync(questionName, pageNumber, pageSize);
            }
        }

        public async Task SearchQuestionsAsync(string questionName, int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                QuestionDtos = await _questionServices.SearchQuestionsByContextMainViewAsync(questionName, pageNumber, pageSize);
                CurrentQuestionName = questionName;
            }
            catch (BaseException.NotFoundException)
            {
                TempData["NotFoundMessage"] = $"Question including '{questionName}' does not exist";
            }
        }

    }
}
