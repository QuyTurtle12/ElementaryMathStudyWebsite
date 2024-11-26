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
        Task<BasePaginatedList<QuizMainViewDto>> SearchQuizzesMainViewByNameAsync(string quizName, int pageNumber, int pageSize);
        Task<QuizViewDto?> GetQuizByChapterOrTopicIdAsync(string? chapterId, string? topicId);
        Task<BasePaginatedList<QuizViewDto>> GetQuizzesAsync(int pageNumber, int pageSize);
        Task<BasePaginatedList<QuizMainViewDto>> GetQuizzesMainViewAsync(int pageNumber, int pageSize);
        Task<QuizMainViewDto> AddQuizAsync(QuizCreateDto dto);
        Task<QuizMainViewDto> UpdateQuizAsync(string quizid, QuizUpdateDto dto);
        Task<BaseResponse<string>> DeleteQuizAsync(string quizId);
    }
}
