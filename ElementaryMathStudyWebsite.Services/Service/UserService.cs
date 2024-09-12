using AutoMapper;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IGenericRepository<User> userRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<User> CreateUserAsync(CreateUserDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            // Use AutoMapper to map the CreateUserDto to a User entity
            var user = _mapper.Map<User>(dto);

            // Hash the password before saving the user
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Status = true; // Default status

            // Add user to the repository
            await _userRepository.InsertAsync(user);
            await _userRepository.SaveAsync();

            return user;
        }

        public async Task<BasePaginatedList<UserResponseDto>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            // Validate pageNumber and pageSize
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10; // Set a default page size if invalid

            // Get the queryable set of users from the repository
            IQueryable<User> query = _userRepository.Entities;

            // Retrieve paginated users from the repository
            var paginatedUsers = await _userRepository.GetPaggingDto(query, pageNumber, pageSize);

            // Map the paginated users to the response DTO
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(paginatedUsers.Items);

            // Return the paginated result with mapped DTOs
            return new BasePaginatedList<UserResponseDto>(userDtos.ToList(), paginatedUsers.TotalItems, paginatedUsers.CurrentPage, paginatedUsers.PageSize);
        }


        // Implement other domain methods here

        public Task<User?> GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> GetUserNameAsync(string userId)
        {
            try
            {
                User user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return string.Empty;
                }

                return user.FullName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task<bool> IsCustomerChildren(string parentId, string studentId)
        {
            User? student = await _userRepository.GetByIdAsync(studentId);
            if (student == null)
            {
                return false;
            }

            string? creatorId = student.CreatedBy;

            if (creatorId.Equals(parentId))
            {
                return true;
            } 
                
            return false;
            
        }
    }
}
