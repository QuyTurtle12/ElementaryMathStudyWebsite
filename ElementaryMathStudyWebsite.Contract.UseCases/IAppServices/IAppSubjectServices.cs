using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppSubjectServices
    {
        // Create a new subject
        Task<SubjectAdminViewDTO> CreateSubjectAsync(SubjectDTO subjectDTO);

        // Search subjects by name
        Task<BasePaginatedList<object>> SearchSubjectAsync(string searchTerm, int pageNumber, int pageSize);

        // Update a subject by ID
        Task<SubjectAdminViewDTO> UpdateSubjectAsync(string id, SubjectDTO subjectDTO);

        // Change subject status by ID
        Task<Subject> ChangeSubjectStatusAsync(string id);

        // Check if subject is existed
        Task<bool> IsValidSubjectAsync(string subjectId);

        // Get subject name
        Task<string> GetSubjectNameAsync(string subjectId);
        Task<SubjectAdminViewDTO> ChangeSubjectStatusAsync(string id);
    }
}
