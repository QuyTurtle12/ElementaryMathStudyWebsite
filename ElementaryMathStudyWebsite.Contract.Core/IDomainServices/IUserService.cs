using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;


namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IUserService
    {
        // Other user-related methods can be added here
        Task<BasePaginatedList<User>> GetAllUsersAsync(int pageNumber, int pageSize);

        Task<User?> GetUserByIdAsync(string userId);
        Task<bool> DisableUserAsync(string userId);

        Task<List<User>> GetAllUsersWithRolesAsync();
        Task<BasePaginatedList<User>> SearchUsersAsync(string? name, bool? status, string? phone, string? email, int pageNumber, int pageSize);
    }
}
