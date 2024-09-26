using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppQuestionServices
    {
        Task<List<QuestionMainViewDto>> GetAllQuestionsMainViewDtoAsync();
        Task<QuestionMainViewDto?> GetQuestionByIdAsync(string questionId);
        Task<List<QuestionViewDto>> SearchQuestionsByContextAsync(string questionContext);
        Task<List<QuestionViewDto>> GetQuestionsByQuizIdAsync(string quizId);
        Task<BasePaginatedList<QuestionMainViewDto>?> GetQuestionsAsync(int pageNumber, int pageSize);
        Task<QuestionMainViewDto> AddQuestionAsync(QuestionCreateDto dto);
        Task<QuestionMainViewDto> UpdateQuestionAsync(string id, QuestionUpdateDto dto);
        Task<QuestionDeleteDto> DeleteQuestionAsync(string id);
    }
}
