using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppQuestionServices
    {
        Task<QuestionViewDto> CreateQuestionAsync(CreateQuestionDto dto);
        Task<Question> GetQuestionByIdAsync(int id);
        Task<IList<QuestionViewDto>> GetAllQuestionsAsync();
        Task<bool> UpdateQuestionAsync(int id, UpdateQuestionDto dto);
        Task<bool> DeleteQuestionAsync(int id);
    }
}
