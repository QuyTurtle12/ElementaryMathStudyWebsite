using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication
{
    public interface IRoleService
    {
        Task<BasePaginatedList<Role>> GetAllRolesAsync(int pageNumber, int pageSize);
        Task<Role> CreateRoleAsync(RequestRole dto);
        Task<Role> UpdateRoleAsync(string roleId, RequestRole dto);
    }
}
