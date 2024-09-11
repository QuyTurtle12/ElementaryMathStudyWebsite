using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.Contract.Services.Interface
{
    public interface ISubjectService
    {
        // Create a new subject
        Task<Subject> CreateSubjectAsync(SubjectDTO subjectDTO);

        // Get all subjects
        Task<IEnumerable<object>> GetAllSubjectsAsync();

        // Get a subject by ID
        Task<Subject> GetSubjectByIDAsync(string id);

        // Search subjects by name
        Task<IEnumerable<object>> SearchSubject(string searchTerm);

        // Update a subject by ID
        Task UpdateSubjectAsync(string id, SubjectDTO subjectDTO);

        // Change subject status by ID
        Task ChangeSubjectStatusAsync(string id);
    }
}
