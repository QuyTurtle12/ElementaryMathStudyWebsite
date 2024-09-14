namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IQuizService
    {
        Task<string?> GetQuizNameAsync(string id);
    }
}
