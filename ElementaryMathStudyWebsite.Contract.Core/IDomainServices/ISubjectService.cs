using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Services.IDomainInterface
{
    public interface ISubjectService
    {

        // Get all subjects
        Task<BasePaginatedList<object>> GetAllSubjectsAsync(int pageNumber, int pageSize, bool isAdmin);

        // Get a subject by ID
        Task<object> GetSubjectByIDAsync(string id, bool isAdmin);

        // Get a subject by ID uu
        Task<Subject> GetSubjectByIDAsync(string id);

    }
}
