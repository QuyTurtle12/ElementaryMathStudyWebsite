using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IChapterService
    {
        Task<BasePaginatedList<Chapter?>> GetChaptersAsync(int pageNumber, int pageSize);

        Task<Chapter?> GetChapterByChapterIdAsync(string id);
        Task<string?> GetChapterNameAsync(string id);
    }
}