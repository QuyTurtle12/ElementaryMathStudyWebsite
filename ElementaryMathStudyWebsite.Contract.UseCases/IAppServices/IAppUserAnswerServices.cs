using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppUserAnswerServices
    {
        Task<ResultProgressDto> CreateUserAnswersAsync(UserAnswerCreateDTO userAnswerCreateDTO);
        Task<UserAnswerDTO> UpdateUserAnswerAsync(string id, UserAnswerDTO userAnswerDTO);
        Task<BasePaginatedList<UserAnswerWithDetailsDTO>> GetAllUserAnswersAsync(int pageNumber, int pageSize);
        Task<UserAnswerDTO> GetUserAnswerByIdAsync(string id);
        Task<BasePaginatedList<UserAnswerWithDetailsDTO>> GetUserAnswersByQuizIdAsync(string quizId);
        Task<ResultProgressDto> CreateUserAnswersUserAsync(UserAnswerCreateDTO userAnswerCreateDTO, string currentUserId);
        Task<List<UserAnswerDTO>> GetUserAnswersByUserAndQuestionsAsync(string userId, List<string> questionIds);
    }
}
