using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.Contract.Services.Interface
{
    public interface ISubjectService
    {
        Task<bool> IsValidSubjectAsync(string subjectId);
        Task<string> GetSubjectNameAsync(string subjectId);
        // Get all subjects
        Task<IEnumerable<object>> GetAllSubjectsAsync();

        // Get a subject by ID
        Task<Subject> GetSubjectByIDAsync(string id);
    }
}
