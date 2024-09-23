using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppQuizServices
    {
        // Get quiz name by quiz id
        //Task<string> GetQuizNameAsync(string quizId);
        // Task<bool> AddQuizAsync(QuizCreateDto dto);
        // Task<QuizViewDto?> GetQuizByIdAsync(string quizId);
        //Task<BasePaginatedList<QuizViewDto?>> GetQuizzDtoAsync(int pageNumber, int pageSize); // get all Quiz with pagination
        //Task<IList<QuizViewDto?>> GetQuizzesAsync(); // get all Quiz exist
        //Task<IList<QuizViewDto>> SearchQuizzesAsync(string? quizName, double? criteria);
        //Task<QuizDetailsDto?> GetQuizByQuizIdAsync(string quizId);
        //Task<QuizViewDto> AddQuizAsync(QuizCreateDto dto);
        //Task<bool> DeleteQuizAsync(string quizId);

        // Task<BasePaginatedList<Quiz?>> GetQuizzesAsync(int pageNumber, int pageSize);
        // Task<bool> IsValidQuizAsync(string quizId);
        // Task<string?> IsGenerallyValidated(string quizName, double? criteria);

        Task<string> GetQuizNameAsync(string quizId);


        Task<List<Quiz>> GetAllQuizzesAsync();


        Task<QuizMainViewDto?> GetQuizByQuizIdAsync(string quizId);
        Task<List<QuizViewDto>> SearchQuizzesByNameAsync(string quizName);
        Task<List<QuizViewDto>> GetQuizzesByChapterOrTopicIdAsync(string? chapterId, string? topicId);
        Task<BasePaginatedList<QuizMainViewDto>?> GetQuizzesAsync(int pageNumber, int pageSize);


        Task<QuizViewDto> AddQuizAsync(CreateQuizDto dto);
        Task<bool> UpdateQuizAsync(UpdateQuizDto dto);
        Task<bool> DeleteQuizAsync(string quizId);

    }
}
