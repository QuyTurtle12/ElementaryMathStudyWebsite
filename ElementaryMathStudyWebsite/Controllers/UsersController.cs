using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Authorization;
using ElementaryMathStudyWebsite.Core.Base;
using Swashbuckle.AspNetCore.Annotations;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;

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

        [HttpGet("profile")]
        public async Task<ActionResult<BaseResponse<UserProfile>>> GetProfile()
        {
            try
            {
                // Get the token from the Authorization header
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                // Extract user ID from the token
                var userId = _tokenService.GetUserIdFromTokenHeader(token);

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                // Fetch user profile using the user ID
                var user = await _userServices.GetUserByIdAsync(userId.ToString());
                var userProfile = _mapper.Map<UserProfile>(user);

                var response = BaseResponse<UserProfile>.OkResponse(userProfile);
                return Ok(response);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }


        [HttpPut]
        [Route("profile/update")]
        [SwaggerOperation(
            Summary = "Authorization: logged in user",
            Description = "Updating a user profile"
            )]
        public async Task<ActionResult<BaseResponse<UpdateProfileDto>>> UpdateProfile([FromBody] UpdateUserDto updateUserDto)
        {
            // Get the token from the Authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Extract user ID from the token
            var userId = _tokenService.GetUserIdFromTokenHeader(token);

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            try
            {
                var user = await _userServices.UpdateUserAsync(userId.ToString(), updateUserDto);
                var UpdateProfileDto = _mapper.Map<UpdateProfileDto>(user);
                UpdateProfileDto.Token = _tokenService.GenerateJwtToken(user);

                var response = BaseResponse<UpdateProfileDto>.OkResponse(UpdateProfileDto);

                return Ok(response);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }

        [HttpGet]
        [Route("get-children")]
        [Authorize(Policy = "Parent")]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "Get page with all children of logged in parent"
            )]
        public async Task<IActionResult> GetChildren([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Get the token from the Authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Extract user ID from the token
            var userId = _tokenService.GetUserIdFromTokenHeader(token);

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            try
            {
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
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
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

                var response = BaseResponse<UserResponseDto>.OkResponse(userResponseDto);

                return Ok(response);

            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
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

                var response = BaseResponse<IEnumerable<UserResponseDto>>.OkResponse(userResponseDtos);

                return Ok(response);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
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

                var response = BaseResponse<BasePaginatedList<UserResponseDto>>.OkResponse(paginatedUserDtos);

                return Ok(response);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
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

                var response = BaseResponse<BasePaginatedList<UserResponseDto>>.OkResponse(paginatedUserDtos);

                return Ok(response);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
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

                var response = BaseResponse<UserResponseDto>.OkResponse(userResponseDto);

                return Ok(response);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
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

                var response = BaseResponse<UserResponseDto>.OkResponse(userResponseDto);

                return Ok(response);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
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
                    var response = BaseResponse<String>.OkResponse("User is disabled");

                    return Ok(response);
                }
                throw new BaseException.BadRequestException("not_found", "User not found");
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }

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
                return BadRequest("User ID is required.");
            }

            try
            {
                var result = await _userServices.DeleteUserAsync(userId);

                if (result)
                {
                    var response = BaseResponse<String>.OkResponse("User is deleted");

                    return Ok(response);
                }
                throw new BaseException.BadRequestException("not_found", "User not found");
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
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

                var response = BaseResponse<BasePaginatedList<UserResponseDto>>.OkResponse(paginatedUserDtos);

                return Ok(response);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }


    }
}
