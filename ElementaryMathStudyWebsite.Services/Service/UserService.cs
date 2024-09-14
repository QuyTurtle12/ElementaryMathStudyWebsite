using AutoMapper;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class UserService : IUserService, IAppUserServices
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        // Constructor
        public UserService(IGenericRepository<User> userRepository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<List<User>> GetAllUsersWithRolesAsync()
        {
            // Define the condition for retrieving users (fetch all users)
            Expression<Func<User, bool>> condition = user => true;

            // Define includes to eagerly load the Role navigation property
            Expression<Func<User, object>>[] includes = new Expression<Func<User, object>>[]
            {
                user => user.Role // Include the Role navigation property
            };

            // Get all users with their roles
            IQueryable<User> query = _userRepository.GetEntitiesWithCondition(condition, includes);

            // Execute the query asynchronously and return the list
            return await query.ToListAsync();
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

            AuditFields(user, isCreating: true);

            // Add user to the repository
            await _userRepository.InsertAsync(user);
            await _userRepository.SaveAsync();

            return user;
        }

        public async Task<BasePaginatedList<User>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            // Validate pageNumber and pageSize
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10; // Set a default page size if invalid

            // Define the condition for retrieving users
            Expression<Func<User, bool>> condition = user => true; // Retrieve all users (no specific condition)

            // Define includes to eagerly load the Role navigation property
            Expression<Func<User, object>>[] includes = new Expression<Func<User, object>>[]
            {
                user => user.Role // Include the Role navigation property
            };

            // Use GetEntitiesWithCondition with includes to get the queryable set of users
            IQueryable<User> query = _userRepository.GetEntitiesWithCondition(condition, includes);

            // Retrieve paginated users from the repository
            var paginatedUsers = await _userRepository.GetPaggingDto(query, pageNumber, pageSize);

            // Return the paginated result with users
            return new BasePaginatedList<User>(paginatedUsers.Items, paginatedUsers.TotalItems, paginatedUsers.CurrentPage, paginatedUsers.PageSize);
        }





        public async Task<User> UpdateUserAsync(string userId, UpdateUserDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            // Define the condition to find the user by ID
            Expression<Func<User, bool>> condition = u => u.Id == userId;

            var user = await _userRepository.FindByConditionWithIncludesAsync(
                condition,
                u => u.Role // Include the Role if needed
                // Add other includes here if needed
            );

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Only update properties if they are not null
            if (dto.FullName != null)
            {
                user.FullName = dto.FullName;
            }

            if (dto.PhoneNumber != null)
            {
                user.PhoneNumber = dto.PhoneNumber;
            }

            if (dto.Gender != null)
            {
                user.Gender = dto.Gender;
            }

            if (dto.Email != null)
            {
                user.Email = dto.Email;
            }

            if (dto.RoleId != null)
            {
                user.RoleId = dto.RoleId;
            }

            if (dto.Username != null)
            {
                user.Username = dto.Username;
            }

            AuditFields(user);

            // Save changes
            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return user;
        }


        // Implement other domain methods here

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            // Define the condition to find the user by ID
            Expression<Func<User, bool>> condition = u => u.Id == userId;

            // Optionally include related entities, if needed
            var user = await _userRepository.FindByConditionWithIncludesAsync(
                condition,
                u => u.Role // Include the Role if needed
                // Add other includes here if needed
            );

            return user;
        }
        
        public async Task<bool> DisableUserAsync(string userId)
        {
            // Fetch the user by ID
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Set the user's status to false to disable the user
            user.Status = false;

            // Set audit fields
            AuditFields(user);

            // Update the user in the repository
            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true; // Return true if the user was successfully disabled
        }

        // Get user name from database
        public async Task<string?> GetUserNameAsync(string userId)
        {
            try
            {
                User? user = await _userRepository.GetByIdAsync(userId);

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

        // Check if the relationship between two users is parents and child
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

        public async Task<BasePaginatedList<User>> SearchUsersAsync(string? name, bool? status, string? phone, string? email, int pageNumber, int pageSize)
        {
            // Define the condition for retrieving users
            Expression<Func<User, bool>> condition = user => true; // Retrieve all users (no specific condition)

            // Define includes to eagerly load the Role navigation property
            Expression<Func<User, object>>[] includes = new Expression<Func<User, object>>[]
            {
                user => user.Role // Include the Role navigation property
            };

            // Use GetEntitiesWithCondition with includes to get the queryable set of users
            IQueryable<User> query = _userRepository.GetEntitiesWithCondition(condition, includes);

            // Apply filters if the corresponding parameters are provided
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.FullName.Contains(name));
            }

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(phone))
            {
                query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(phone));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email != null && u.Email.Contains(email));
            }

            // Execute the query asynchronously and return the result

            // Calculate total count before pagination
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Create a paginated list
            return new BasePaginatedList<User>(
                items,
                totalItems,
                pageNumber,
                pageSize
            );
        }


        public void AuditFields(BaseEntity entity, bool isCreating = false)
        {
            // Retrieve the JWT token from the Authorization header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var currentUserId = _tokenService.GetUserIdFromTokenHeader(token);

            // If creating a new entity, set the CreatedBy field
            if (isCreating)
            {
                entity.CreatedBy = currentUserId.ToString(); // Set the creator's ID
            }

            // Always set LastUpdatedBy and LastUpdatedTime fields
            entity.LastUpdatedBy = currentUserId.ToString().ToUpper(); // Set the current user's ID
            entity.LastUpdatedTime = DateTime.UtcNow;
        }

    }
}
