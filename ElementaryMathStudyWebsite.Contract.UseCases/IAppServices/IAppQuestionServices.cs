using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppQuestionServices
    {
        Task<List<QuestionMainViewDto>> GetAllQuestionsMainViewDtoAsync();

        Task<QuestionMainViewDto> GetQuestionByIdAsync(string questionId);
        Task<List<QuestionViewDto>> GetQuestionsByQuizIdAsync(string quizId);
        Task<BasePaginatedList<QuestionViewDto>> GetQuestionsByQuizIdAsync(string id, int pageNumber, int pageSize);
        Task<BasePaginatedList<QuestionMainViewDto>> GetQuestionsMainViewByQuizIdAsync(string id, int pageNumber, int pageSize);

        Task<List<QuestionViewDto>> SearchQuestionsByContextAsync(string questionContext);
        Task<BasePaginatedList<QuestionMainViewDto>> SearchQuestionsByContextMainViewAsync(string questionContext, int pageNumber, int pageSize);

        Task<BasePaginatedList<QuestionViewDto>> GetQuestionsAsync(int pageNumber, int pageSize);
        Task<BasePaginatedList<QuestionMainViewDto>> GetQuestionsMainViewAsync(int pageNumber, int pageSize);


        Task<IList<Question>> GetQuestionsWithOptionsEntitiesByQuizIdAsync(string quizId);

        Task<BaseResponse<string>?> AddQuestionAsync(List<QuestionCreateDto> dtos, User? currentUser);
        Task<QuestionMainViewDto?> UpdateQuestionAsync(string id, QuestionUpdateDto dto, User currentUser);
        Task<BaseResponse<string>> DeleteQuestionAsync(string questionId);
    }
}
