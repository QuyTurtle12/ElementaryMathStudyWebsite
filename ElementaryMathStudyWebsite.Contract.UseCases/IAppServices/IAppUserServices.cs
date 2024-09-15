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
        Task<string?> GetUserNameAsync(string userId);

        // Check if the relationship between two users is parents and child
        Task<bool> IsCustomerChildren(string parentId, string studentId);
        void AuditFields(BaseEntity entity, bool isCreating = false);
    }
}
