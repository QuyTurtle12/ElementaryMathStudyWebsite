using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;

        public UserService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<User> CreateUserAsync(CreateUserDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            // Map DTO to User entity
            var user = new User
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Gender = dto.Gender,
                Email = dto.Email,
                RoleId = dto.RoleId,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Status = true // Default status
            };

            // Add user to the repository
            await _userRepository.InsertAsync(user);
            await _userRepository.SaveAsync();

            return user;
        }

        // Implement other domain methods here
    }
}
