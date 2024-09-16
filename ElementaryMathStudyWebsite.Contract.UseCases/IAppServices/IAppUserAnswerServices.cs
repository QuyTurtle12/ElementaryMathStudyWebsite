using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppUserAnswerServices
    {
        Task<UserAnswer> CreateUserAnswerAsync(UserAnswer userAnswer);

        Task UpdateUserAnswerAsync(UserAnswer userAnswer);
    }
}
