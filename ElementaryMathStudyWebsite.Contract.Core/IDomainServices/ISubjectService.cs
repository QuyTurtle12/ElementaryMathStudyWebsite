namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface ISubjectService
    {
        Task<bool> IsValidSubjectAsync(string subjectId);
        Task<string> GetSubjectNameAsync(string subjectId);
    }
}
