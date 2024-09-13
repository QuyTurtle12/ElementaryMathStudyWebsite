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
        Task<IList<Quiz?>> GetQuizzesAsync(); // get all Quiz with pagination

       // Task<BasePaginatedList<Quiz?>> GetQuizzesAsync(int pageNumber, int pageSize);
       // Task<bool> IsValidQuizAsync(string quizId);
       // Task<string?> IsGenerallyValidated(string quizName, double? criteria);
    }
}
