using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IUserService
    {
        // Get user by Id
        Task<User?> GetUserByIdAsync(string userId);
    }
}
