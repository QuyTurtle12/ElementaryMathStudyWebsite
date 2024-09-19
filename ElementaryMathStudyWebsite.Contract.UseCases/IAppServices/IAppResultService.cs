using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppResultService
    {
        // Calculate the latest student score
        Task<double> CalculateLatestStudentScoreAsync(string quizId, string studentId);

        // Get a list of student grade of specific quiz
        Task<BasePaginatedList<double>> GetStudentResultListAsync(string quizId, string studentId);

        // Check if the student passed the quiz
        Task<bool> IsPassedTheQuizAsync(string quizId, string studentId);
    }
}
