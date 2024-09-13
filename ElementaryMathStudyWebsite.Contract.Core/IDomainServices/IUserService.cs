using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IUserService
    {
        Task<string?> GetUserNameAsync(string userId);

        Task<User?> GetUserByIdAsync(string userId);

        Task<bool> IsCustomerChildren(string parentId, string studentId);
    }
}
