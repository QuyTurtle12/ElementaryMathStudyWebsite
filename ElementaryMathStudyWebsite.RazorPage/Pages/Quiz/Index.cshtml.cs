using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.Quiz
{
    public class IndexModel : PageModel
    {
        private readonly IAppQuizServices _quizService;
        public string CurrentQuizName = string.Empty;

        public IndexModel(IAppQuizServices quizService)
        {
            _quizService = quizService;
        }

        public BasePaginatedList<QuizMainViewDto> QuizDtos { get; set; } = new BasePaginatedList<QuizMainViewDto>(new List<QuizMainViewDto>(), 0, 1, 10);


        public async Task OnGetAsync(string quizName, int pageNumber = 1, int pageSize = 10)
        {
            CurrentQuizName = quizName;

            if (string.IsNullOrEmpty(quizName))
            {
                QuizDtos = await _quizService.GetQuizzesMainViewAsync(pageNumber, pageSize);
            }
            else
            {
                await SearchQuizzesAsync(quizName, pageNumber, pageSize);
            }
        }

        public async Task SearchQuizzesAsync(string quizName, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                QuizDtos = await _quizService.SearchQuizzesMainViewByNameAsync(quizName, pageNumber, pageSize);
                CurrentQuizName = quizName;
            }
            catch (BaseException.NotFoundException ex)
            {
                TempData["NotFoundMessage"] = ex.Message;
            }
        }


    }

}
