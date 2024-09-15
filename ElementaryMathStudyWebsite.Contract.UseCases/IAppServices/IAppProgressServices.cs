using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppProgressServices
    {
        // Get a list of subject progress that one student of one parent currently studying
        public Task<BasePaginatedList<ProgressViewDto>> GetStudentProgressesDtoAsync(string studentId, int pageNumber, int pageSize);

        // Get a list of subject progress that all students of one parent currently studying
        public Task<BasePaginatedList<ProgressViewDto>> GetAllStudentProgressesDtoAsync(string parentId, int pageNumber, int pageSize);

        // Add new progress that student has just assigned to study a subject
        public Task<bool> AddSubjectProgress(Progress studentProgress);

        // Check if student is currently studying a specific subject
        public bool IsCurrentlyStudyingThisSubject(string studentId, string subjectId);

    }
}
