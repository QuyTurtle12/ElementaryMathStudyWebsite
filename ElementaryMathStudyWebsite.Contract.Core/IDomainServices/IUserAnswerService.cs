using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IUserAnswerService
    {
        Task<BasePaginatedList<object>> GetAllUserAnswersAsync(int pageNumber, int pageSize);

        Task<UserAnswerDTO> GetUserAnswerByIdAsync(string id);
    }
}
