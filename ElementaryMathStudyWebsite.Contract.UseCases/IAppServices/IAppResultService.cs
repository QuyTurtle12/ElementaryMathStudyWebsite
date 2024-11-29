using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppResultService
    {
        // Calculate the latest student score
        Task<double> CalculateLatestStudentScoreAsync(string quizId, string studentId);

        // Get a list of student grade of specific quiz
        Task<BasePaginatedList<ResultViewDto>> GetStudentResultListAsync(User currentUser, string quizId, int pageNumber, int pageSize);

        // Check if the student passed the quiz
        Task<bool> IsPassedTheQuizAsync(string quizId, string studentId);

        // Add student result to database
        Task<ResultProgressDto> AddStudentResultAsync(ResultCreateDto result);

        // Get the children latest result of assigned subject
        Task<ResultParentViewDto> GetChildrenLatestResultAsync(string studentId);

        // Get a quiz id by chapter or topic Id 
        Task<string> GetQuizIdByChapterOrTopicId(string chapterOrTopicId);
        Task<ResultProgressDto> AddStudentResultUserAsync(ResultCreateDto dto, string studentId);
    }
}
