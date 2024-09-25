using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication
{
    public interface IRoleService
    {
        Task<BasePaginatedList<Role>> GetAllRolesAsync(int pageNumber, int pageSize);
    }
}
