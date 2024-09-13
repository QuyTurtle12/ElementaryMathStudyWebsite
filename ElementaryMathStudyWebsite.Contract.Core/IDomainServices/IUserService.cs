using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;


namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface IUserService
    {
        //Task<User> CreateUserAsync(CreateUserDto dto);
        // Other user-related methods can be added here
        Task<BasePaginatedList<UserResponseDto>> GetAllUsersAsync(int pageNumber, int pageSize);
        // Get user by Id
        Task<User?> GetUserByIdAsync(string userId);
    }
}
