using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Services.IDomainInterface
{
    public interface ISubjectService
    {
<<<<<<< HEAD

=======
        Task<bool> IsValidSubjectAsync(string subjectId);
        Task<string> GetSubjectNameAsync(string subjectId);
>>>>>>> bao
        // Get all subjects
        Task<BasePaginatedList<object>> GetAllSubjectsAsync(int pageNumber, int pageSize, bool isAdmin);

        // Get a subject by ID
<<<<<<< HEAD
        Task<Subject> GetSubjectByIDAsync(string id);

=======
        Task<object> GetSubjectByIDAsync(string id, bool isAdmin);
>>>>>>> bao
    }
}
