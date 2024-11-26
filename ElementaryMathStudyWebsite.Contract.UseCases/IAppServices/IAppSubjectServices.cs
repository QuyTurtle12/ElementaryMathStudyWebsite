using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppSubjectServices
    {
        // Create a new subject
        Task<SubjectAdminViewDTO> CreateSubjectAsync(SubjectDTO subjectDTO);

        // Search subjects by name
        Task<BasePaginatedList<object>> SearchSubjectAsync(string searchTerm, double lowestPrice,
                double highestPrice, int pageNumber, int pageSize);

        Task<BasePaginatedList<object>> SearchSubjectAdminAsync(string searchTerm, double lowestPrice,
                double highestPrice, bool? status, int pageNumber, int pageSize);

        // Update a subject by ID
        Task<SubjectAdminViewDTO> UpdateSubjectAsync(string id, SubjectUpdateDTO subjectUpdateDTO);

        // Change subject status by ID
        //Task<SubjectAdminViewDTO> ChangeSubjectStatusAsync(string id);

        // Check if subject is existed
        Task<bool> IsValidSubjectAsync(string subjectId);

        // Get subject name
        Task<string> GetSubjectNameAsync(string subjectId);

        // Get all subjects
        Task<BasePaginatedList<object>> GetAllSubjectsAsync(int pageNumber, int pageSize, bool isAdmin);

        // Get a subject by ID
        Task<object> GetSubjectByIDAsync(string id, bool isAdmin);

        // Get a subject by ID uu
        Task<Subject> GetSubjectByIDAsync(string id);
        Task<bool> CheckCompleteQuizExistAsync(string subjectId, string quizId);
        Task SoftDeleteSubjectAsync(string id);
        Task RestoreSubjectAsync(string id);
        Task AuditSubjectAsync(Subject subject, string currentUserId, bool isCreated);
        Task<Subject> SearchSubjectExactAsync(string searchTerm);
    }
}
