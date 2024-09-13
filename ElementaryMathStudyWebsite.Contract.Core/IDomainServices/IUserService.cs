using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;


namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IUserService
    {
        //Task<User> CreateUserAsync(CreateUserDto dto);
        // Other user-related methods can be added here
        Task<BasePaginatedList<User>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<string?> GetUserNameAsync(string userId);

        Task<User?> GetUserByIdAsync(string userId);

        Task<bool> IsCustomerChildren(string parentId, string studentId);
        Task<bool> DisableUserAsync(string userId);

        //Task<User> UpdateUserAsync(string userId, UpdateUserDto dto);
    }
}
