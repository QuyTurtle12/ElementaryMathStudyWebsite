using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IOptionService
    {

        Task<BasePaginatedList<Option?>> GetOptions(int pageNumber, int pageSize);
        
        // Get option with all properties
        Task<Option?> GetOptionById(string optionId);

    }
}
