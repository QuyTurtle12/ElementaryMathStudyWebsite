using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.Contract.Services.IDomainInterface
{
    public interface ISubjectService
    {

        // Get all subjects
        Task<IEnumerable<object>> GetAllSubjectsAsync();

        // Get a subject by ID
        Task<Subject> GetSubjectByIDAsync(string id);

    }
}
