using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppQuestionServices
    {
        Task<List<Question>> GetAllQuestionsAsync();
        Task<QuestionViewDto?> GetQuestionByIdAsync(string questionId);
        Task<List<QuestionViewDto>> SearchQuestionsByContextAsync(string questionContext);
        Task<List<QuestionViewDto>> GetQuestionsByQuizIdAsync(string quizId);
        Task<BasePaginatedList<QuestionViewDto>?> GetQuestionsAsync(int pageNumber, int pageSize);
        //Task<BasePaginatedList<QuestionViewDto>> GetQuestionsPagedAsync(int pageNumber, int pageSize);
    }
}
