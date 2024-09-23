using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppUserServices
    {
        Task<User> CreateUserAsync(CreateUserDto dto);
        Task<User> UpdateUserAsync(string userId, UpdateUserDto dto);
        // Get user name by id
        Task<string> GetUserNameAsync(string userId);

        // Check if the relationship between two users is parents and child
        Task<bool> IsCustomerChildren(string parentId, string studentId);
        void AuditFields(BaseEntity entity, bool isCreating = false, bool isDisable = false);
        string GetActionUserId();

        // Other user-related methods can be added here
        Task<BasePaginatedList<User>> GetAllUsersAsync(int pageNumber, int pageSize);

        Task<User?> GetUserByIdAsync(string userId);
        Task<User> GetCurrentUserAsync();
        Task<bool> DisableUserAsync(string userId);

        Task<List<User>> GetAllUsersWithRolesAsync();
        Task<BasePaginatedList<User>> SearchUsersAsync(string? name, bool? status, string? phone, string? email, int pageNumber, int pageSize);
    }
}
