using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppQuizServices
    {

        Task<string> GetQuizNameAsync(string quizId);

        Task<List<QuizMainViewDto>> GetAllQuizzesAsync();


        Task<QuizMainViewDto> GetQuizByQuizIdAsync(string quizId);
        Task<List<QuizViewDto>> SearchQuizzesByNameAsync(string quizName);
        Task<List<QuizViewDto>> GetQuizzesByChapterOrTopicIdAsync(string? chapterId, string? topicId);
        Task<BasePaginatedList<QuizMainViewDto>> GetQuizzesAsync(int pageNumber, int pageSize);

        Task<QuizMainViewDto> AddQuizAsync(QuizCreateDto dto);
        Task<QuizMainViewDto> UpdateQuizAsync(string quizid, QuizUpdateDto dto);
        Task<bool> DeleteQuizAsync(string quizId);
    }
}
