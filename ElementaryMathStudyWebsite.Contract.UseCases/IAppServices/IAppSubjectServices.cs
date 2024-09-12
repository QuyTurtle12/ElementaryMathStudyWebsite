using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppSubjectServices
    {
        // Create a new subject
        Task<Subject> CreateSubjectAsync(SubjectDTO subjectDTO);

        // Search subjects by name
        Task<IEnumerable<object>> SearchSubjectAsync(string searchTerm);

        // Update a subject by ID
        Task<Subject> UpdateSubjectAsync(string id, SubjectDTO subjectDTO);

        // Change subject status by ID
        Task<Subject> ChangeSubjectStatusAsync(string id);
    }
}
