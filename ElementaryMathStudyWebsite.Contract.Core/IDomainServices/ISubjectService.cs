using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Services.Interface
{
    public interface ISubjectService
    {
        Task<bool> IsValidSubjectAsync(string subjectId);
        Task<string> GetSubjectNameAsync(string subjectId);
        // Get all subjects
        Task<BasePaginatedList<object>> GetAllSubjectsAsync(int pageNumber, int pageSize, bool isAdmin);

        // Get a subject by ID
        Task<object> GetSubjectByIDAsync(string id, bool isAdmin);
    }
}
