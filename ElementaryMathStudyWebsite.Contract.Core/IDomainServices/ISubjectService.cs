using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.Contract.Core.IDomainServices
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
