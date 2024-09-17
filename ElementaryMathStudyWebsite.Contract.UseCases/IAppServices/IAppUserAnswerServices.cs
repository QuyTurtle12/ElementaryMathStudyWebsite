using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppUserAnswerServices
    {
        Task<UserAnswerDTO> CreateUserAnswerAsync(UserAnswerDTO userAnswerDTO);
        Task<UserAnswerDTO> UpdateUserAnswerAsync(string id, UserAnswerDTO userAnswerDTO);
        Task<BasePaginatedList<object>> GetAllUserAnswersAsync(int pageNumber, int pageSize);
        Task<UserAnswerDTO> GetUserAnswerByIdAsync(string id);
    }
}
