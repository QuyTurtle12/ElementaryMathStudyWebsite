using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using ElementaryMathStudyWebsite.Repositories.DTOs;

namespace ElementaryMathStudyWebsite.Contract.Services.Interface
{
    public interface ISubjectService
    {
        Task<IEnumerable<object>> GetAllSubjectsAsync();
        Task<Subject> GetSubjectByIDAsync(string id);
        Task<Subject> CreateSubjectAsync(SubjectDTO subjectDTO);
        Task UpdateSubjectAsync(string id, SubjectDTO subjectDTO);
        Task ChangeSubjectStatusAsync(string id);
        Task<IEnumerable<object>> SearchSubject(string searchTerm);
    }
}
