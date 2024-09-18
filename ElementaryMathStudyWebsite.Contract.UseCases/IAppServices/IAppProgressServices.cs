using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppProgressServices
    {
        // Get a list of subject progress that one student of one parent currently studying
        Task<BasePaginatedList<ProgressViewDto>> GetStudentProgressesDtoAsync(string studentId, int pageNumber, int pageSize);

        // Get a list of subject progress that all students of one parent currently studying
        Task<BasePaginatedList<ProgressViewDto>> GetAllStudentProgressesDtoAsync(string parentId, int pageNumber, int pageSize);

        // Add new progress that student has just assigned to study a subject
        Task<bool> AddSubjectProgressAsync(Progress studentProgress);

        // Check if student is currently studying a specific subject
        Task<bool> IsCurrentlyStudyingThisSubjectAsync(string studentId, string subjectId);

        // Get the student grade
        Task<double> GetStudentGradeAsync(string quizId, string studentId);

        // Check if the student passed the quiz
        Task<bool> IsPassedTheQuizAsync(string quizId, string studentId);

        // Identify which subject does the quiz belong to
        Task<string> GetSubjectIdFromQuizIdAsync(string quizId);

        // General validation
        Task<string> IsGenerallyValidatedAsync(string quizId, string studentId);  

        // Check if student has been assigned to a specific subject
        Task<bool> HasStudentBeenAssignedToTheSubjectAsync(string studentId, string subjectId);

        // Get a list of assigned subject of specific student
        Task<BasePaginatedList<AssignedSubjectDto>?> GetAssignedSubjectListAsync(int pageNumber, int pageSize);
    }
}
