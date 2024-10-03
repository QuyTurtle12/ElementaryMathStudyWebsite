using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAppUserServices _userServices;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UsersController(IAppUserServices userServices, IMapper mapper, ITokenService tokenService)
        {
            _userServices = userServices;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Retrieves the profile of the logged-in user.
        /// </summary>
        /// <returns>Returns the profile of the logged-in user.</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<BaseResponse<UserProfile>>> GetProfile()
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId) || userId == Guid.Empty)
            {
                return Unauthorized();
            }

            // Fetch user profile using the user ID
            var user = await _userServices.GetUserByIdAsync(userId.ToString());
            var userProfile = _mapper.Map<UserProfile>(user);

            var response = BaseResponse<UserProfile>.OkResponse(userProfile);
            return Ok(response);
        }

        /// <summary>
        /// Updates the profile of the logged-in user.
        /// </summary>
        /// <param name="updateUserDto">The data used to update the user's profile.</param>
        /// <returns>Returns the updated user profile along with a new JWT token.</returns>
        [HttpPut]
        [Route("profile/update")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Authorization: logged in user",
            Description = "Updating a user profile"
            )]
        public async Task<ActionResult<BaseResponse<UpdateProfileDto>>> UpdateProfile([FromBody] RequestUpdateProfileDto updateUserDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId) || userId == Guid.Empty)
            {
                return Unauthorized();
            }
            var user = await _userServices.UpdateProfileAsync(userId.ToString(), updateUserDto);
            var UpdateProfileDto = _mapper.Map<UpdateProfileDto>(user);
            UpdateProfileDto.Token = _tokenService.GenerateJwtToken(user);

            var response = BaseResponse<UpdateProfileDto>.OkResponse(UpdateProfileDto);

            return Ok(response);

        }

        /// <summary>
        /// Retrieves a paginated list of all children associated with the logged-in parent.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination (default is 1).</param>
        /// <param name="pageSize">The page size for pagination (default is 10).</param>
        /// <returns>Returns a paginated list of children for the parent.</returns>
        [HttpGet]
        [Route("get-children")]
        [Authorize(Policy = "Parent")]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "Get page with all children of logged in parent"
            )]
        public async Task<IActionResult> GetChildren([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId) || userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var paginatedUsers = await _userServices.GetChildrenOfParentAsync(userId.ToString(), pageNumber, pageSize);

            // Map users to UserResponseDto
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(paginatedUsers.Items);

            // Create a new paginated list of UserResponseDto
            var paginatedUserDtos = new BasePaginatedList<UserResponseDto>(
                userDtos.ToList(),
                paginatedUsers.TotalItems,
                paginatedUsers.CurrentPage,
                paginatedUsers.PageSize
            );

            var response = BaseResponse<BasePaginatedList<UserResponseDto>>.OkResponse(paginatedUserDtos);

            return Ok(response);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="createUserDto">The user data required for creation.</param>
        /// <returns>Returns the created user along with its ID.</returns>
        [HttpPost]
        [Route("create")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Creating new user"
            )]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null)
            {
                throw new BaseException.BadRequestException("invalid_argument", "User data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userServices.CreateUserAsync(createUserDto);
            var userResponseDto = _mapper.Map<UserResponseDto>(user);

            var response = BaseResponse<UserResponseDto>.OkResponse(userResponseDto);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a list of all users with their associated roles.
        /// </summary>
        /// <returns>Returns a list of users with their roles.</returns>
        [HttpGet]
        [Route("all-list")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Manager",
            Description = "Get a list with all users"
            )]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {

            var users = await _userServices.GetAllUsersWithRolesAsync();

            // Map the users to UserResponseDto
            var userResponseDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);

            var response = BaseResponse<IEnumerable<UserResponseDto>>.OkResponse(userResponseDtos);

            return Ok(response);
        }


        /// <summary>
        /// Retrieves all users with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of users per page (default is 10).</param>
        /// <returns>Returns a paginated list of users.</returns>
        [HttpGet]
        [Route("all-pagination")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Manager",
            Description = "Get page with all users "
            )]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {

            var paginatedUsers = await _userServices.GetAllUsersAsync(pageNumber, pageSize);

            // Map users to UserResponseDto
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(paginatedUsers.Items);

            // Create a new paginated list of UserResponseDto
            var paginatedUserDtos = new BasePaginatedList<UserResponseDto>(
                userDtos.ToList(),
                paginatedUsers.TotalItems,
                paginatedUsers.CurrentPage,
                paginatedUsers.PageSize
            );

            var response = BaseResponse<BasePaginatedList<UserResponseDto>>.OkResponse(paginatedUserDtos);

            return Ok(response);
        }

        /// <summary>
        /// Searches users based on provided filters such as name, status, phone, and email with pagination.
        /// </summary>
        /// <param name="name">The name of the user to search for (optional).</param>
        /// <param name="status">The status of the user (optional).</param>
        /// <param name="phone">The phone number of the user (optional).</param>
        /// <param name="email">The email of the user (optional).</param>
        /// <param name="pageNumber">The page number for pagination (default is 1).</param>
        /// <param name="pageSize">The page size for pagination (default is 10).</param>
        /// <returns>Returns a paginated list of users based on the search criteria.</returns>
        [HttpGet]
        [Route("search")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Manager",
            Description = "Search users based on name, status, phone, and email with pagination."
        )]
        public async Task<IActionResult> SearchUsers([FromQuery] string? name, [FromQuery] bool? status, [FromQuery] string? phone, [FromQuery] string? email, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {

            var paginatedUsers = await _userServices.SearchUsersAsync(name, status, phone, email, pageNumber, pageSize);

            // Map users to UserResponseDto
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(paginatedUsers.Items);

            // Create a new paginated list of UserResponseDto
            var paginatedUserDtos = new BasePaginatedList<UserResponseDto>(
                userDtos.ToList(),
                paginatedUsers.TotalItems,
                paginatedUsers.CurrentPage,
                paginatedUsers.PageSize
            );

            var response = BaseResponse<BasePaginatedList<UserResponseDto>>.OkResponse(paginatedUserDtos);

            return Ok(response);

        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>Returns the user details.</returns>
        [HttpGet]
        [Route("get/{userId}")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Get a user by id"
            )]
        public async Task<IActionResult> GetUserById(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new BaseException.BadRequestException("invalid_argument", "User ID is required.");
            }
            var user = await _userServices.GetUserByIdAsync(userId);

            var userResponseDto = _mapper.Map<UserResponseDto>(user);

            var response = BaseResponse<UserResponseDto>.OkResponse(userResponseDto);

            return Ok(response);
        }

        /// <summary>
        /// Updates user information.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="updateUserDto">The updated user data.</param>
        /// <returns>Returns the updated user details.</returns>
        [HttpPut]
        [Route("update/{userId}")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Updating a user"
            )]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new BaseException.BadRequestException("invalid_argument", "User ID is required.");
            }
            if (updateUserDto == null)
            {
                throw new BaseException.BadRequestException("invalid_argument", "User data is required.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userServices.UpdateUserAsync(userId, updateUserDto);
            var userResponseDto = _mapper.Map<UserResponseDto>(user);
            var response = BaseResponse<UserResponseDto>.OkResponse(userResponseDto);
            return Ok(response);
        }

        /// <summary>
        /// Disables a user.
        /// </summary>
        /// <param name="userId">The ID of the user to disable.</param>
        /// <returns>Returns a success or failure message.</returns>
        [HttpPatch]
        [Route("disable/{userId}")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Disabling a user"
            )]
        public async Task<IActionResult> DisableUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new BaseException.BadRequestException("invalid_argument", "User ID is required.");
            }

            var result = await _userServices.DisableUserAsync(userId);

            if (result)
            {
                var response = BaseResponse<String>.OkResponse("User is disabled");

                return Ok(response);
            }
            throw new BaseException.CoreException("unsuccess", "Disable unsuccessfully");
        }

        /// <summary>
        /// Deletes a user based on the provided user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to be deleted.</param>
        /// <returns>Returns a confirmation message if the user is successfully deleted.</returns>
        [HttpDelete]
        [Route("delete/{userId}")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Deleting a user"
            )]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new BaseException.BadRequestException("invalid_argument", "User ID is required.");
            }

            var result = await _userServices.DeleteUserAsync(userId);

            if (result)
            {
                var response = BaseResponse<String>.OkResponse("User is deleted");
                return Ok(response);
            }
            throw new BaseException.CoreException("unsuccess", "Delete unsuccessfully");
        }

        /// <summary>
        /// Retrieves all children of a given parent with pagination.
        /// </summary>
        /// <param name="parentId">The ID of the parent user.</param>
        /// <param name="pageNumber">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of users per page (default is 10).</param>
        /// <returns>Returns a paginated list of users who are children of the specified parent.</returns>
        [HttpGet]
        [Route("children/{parentId}")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Get children of a parent user with pagination."
        )]
        public async Task<IActionResult> GetChildrenOfParent(string parentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                throw new BaseException.BadRequestException("invalid_argument", "Parent ID is required.");
            }

            var paginatedUsers = await _userServices.GetChildrenOfParentAsync(parentId, pageNumber, pageSize);

            // Map users to UserResponseDto
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(paginatedUsers.Items);

            // Create a new paginated list of UserResponseDto
            var paginatedUserDtos = new BasePaginatedList<UserResponseDto>(
                userDtos.ToList(),
                paginatedUsers.TotalItems,
                paginatedUsers.CurrentPage,
                paginatedUsers.PageSize
            );

            var response = BaseResponse<BasePaginatedList<UserResponseDto>>.OkResponse(paginatedUserDtos);

            return Ok(response);
        }
    }
}
