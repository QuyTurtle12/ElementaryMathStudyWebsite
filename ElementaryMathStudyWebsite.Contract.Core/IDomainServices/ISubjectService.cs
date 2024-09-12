using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.Contract.Services.IDomainInterface
{
    public interface ISubjectService
    {
        // Check if subject is existed
        Task<bool> IsValidSubjectAsync(string subjectId);

        // Get subject name
        Task<string> GetSubjectNameAsync(string subjectId);

        // Get all subjects
        Task<IEnumerable<object>> GetAllSubjectsAsync();

        // Get a subject by ID
        Task<Subject> GetSubjectByIDAsync(string id);

    }
}
