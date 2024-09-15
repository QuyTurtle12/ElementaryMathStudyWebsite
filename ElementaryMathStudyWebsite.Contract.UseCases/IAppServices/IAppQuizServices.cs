using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppQuizServices
    {
       // Task<bool> AddQuizAsync(QuizCreateDto dto);
       // Task<QuizViewDto?> GetQuizByIdAsync(string quizId);
        Task<BasePaginatedList<QuizViewDto?>> GetQuizzDtoAsync(int pageNumber, int pageSize); // get all Quiz with pagination
        Task<IList<QuizViewDto?>> GetQuizzesAsync(); // get all Quiz exist
        Task<IList<QuizViewDto>> SearchQuizzesAsync(string? quizName, double? criteria);
        Task<QuizDetailsDto?> GetQuizByQuizIdAsync(string quizId);
        Task<QuizViewDto> AddQuizAsync(QuizCreateDto dto);
        Task<bool> DeleteQuizAsync(string quizId);

       // Task<BasePaginatedList<Quiz?>> GetQuizzesAsync(int pageNumber, int pageSize);
       // Task<bool> IsValidQuizAsync(string quizId);
       // Task<string?> IsGenerallyValidated(string quizName, double? criteria);
    }
}
