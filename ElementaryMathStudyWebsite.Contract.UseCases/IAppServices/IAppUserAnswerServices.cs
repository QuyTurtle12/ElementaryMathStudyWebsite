using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppUserAnswerServices
    {
        Task<UserAnswer> CreateUserAnswerAsync(UserAnswer userAnswer);
        Task UpdateUserAnswerAsync(UserAnswer userAnswer);
        Task<BasePaginatedList<object>> GetAllUserAnswersAsync(int pageNumber, int pageSize);
        Task<Object> GetUserAnswerByIdAsync(string id);
    }
}
