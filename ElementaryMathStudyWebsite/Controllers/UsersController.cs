using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Authorization;
using ElementaryMathStudyWebsite.Core.Base;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAppUserServices _userServices;
        private readonly IMapper _mapper;

        public UsersController(IAppUserServices userServices, IMapper mapper)
        {
            _userServices = userServices;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="createUserDto">The user data required for creation.</param>
        /// <returns>Returns the created user along with its ID.</returns>
        /// <response code="201">User created successfully.</response>
        /// <response code="400">Invalid user data provided.</response>
        /// <response code="500">Internal server error.</response>
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
                return BadRequest("User data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userServices.CreateUserAsync(createUserDto);
                var userResponseDto = _mapper.Map<UserResponseDto>(user); 

                return CreatedAtAction(nameof(CreateUser), new { id = userResponseDto.Id }, userResponseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a list of all users with their associated roles.
        /// </summary>
        /// <returns>Returns a list of users with their roles.</returns>
        /// <response code="200">List of users retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("all-list")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Get a list with all users"
            )]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            try
            {
                var users = await _userServices.GetAllUsersWithRolesAsync();

                // Map the users to UserResponseDto
                var userResponseDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);

                return Ok(userResponseDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        /// <summary>
        /// Retrieves all users with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of users per page (default is 10).</param>
        /// <returns>Returns a paginated list of users.</returns>
        /// <response code="200">List of users retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("all-pagination")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Get page with all users "
            )]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
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

                return Ok(paginatedUserDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("search")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Search users based on name, status, phone, and email with pagination."
        )]
        public async Task<IActionResult> SearchUsers([FromQuery] string? name, [FromQuery] bool? status, [FromQuery] string? phone, [FromQuery] string? email, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
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

                return Ok(paginatedUserDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }



        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>Returns the user details.</returns>
        /// <response code="200">User found and details returned.</response>
        /// <response code="400">Invalid user ID provided.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
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
                return BadRequest("User ID is required.");
            }

            try
            {
                var user = await _userServices.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var userResponseDto = _mapper.Map<UserResponseDto>(user);
                return Ok(userResponseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates user information.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="updateUserDto">The updated user data.</param>
        /// <returns>Returns the updated user details.</returns>
        /// <response code="200">User updated successfully.</response>
        /// <response code="400">Invalid user ID or data provided.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
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
                return BadRequest("User ID is required.");
            }

            if (updateUserDto == null)
            {
                return BadRequest("User data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userServices.UpdateUserAsync(userId, updateUserDto);
                var userResponseDto = _mapper.Map<UserResponseDto>(user);

                return Ok(userResponseDto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Disables a user.
        /// </summary>
        /// <param name="userId">The ID of the user to disable.</param>
        /// <returns>Returns a success or failure message.</returns>
        /// <response code="200">User successfully disabled.</response>
        /// <response code="400">Invalid user ID provided.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
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
                return BadRequest("User ID is required.");
            }

            try
            {
                var result = await _userServices.DisableUserAsync(userId);

                if (result)
                {
                    return Ok("User has been successfully disabled.");
                }
                return NotFound("User not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all children of a given parent with pagination.
        /// </summary>
        /// <param name="parentId">The ID of the parent user.</param>
        /// <param name="pageNumber">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of users per page (default is 10).</param>
        /// <returns>Returns a paginated list of users who are children of the specified parent.</returns>
        /// <response code="200">List of children retrieved successfully.</response>
        /// <response code="400">Invalid parent ID provided.</response>
        /// <response code="500">Internal server error.</response>
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
                return BadRequest("Parent ID is required.");
            }

            try
            {
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

                return Ok(paginatedUserDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


    }
}
