using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.Quiz
{
    public class IndexModel : PageModel
    {
        private readonly IAppQuizServices _quizService;
        private readonly IMapper _mapper;

        public IndexModel(IAppQuizServices quizService, IMapper mapper)
        {
            _quizService = quizService;
            _mapper = mapper;
        }

        public BasePaginatedList<QuizMainViewDto> QuizDtos { get; set; } = new BasePaginatedList<QuizMainViewDto>(new List<QuizMainViewDto>(), 0, 1, 10);

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
            QuizDtos = await _quizService.GetQuizzesMainViewAsync(pageNumber, pageSize);
        }
    }

}
