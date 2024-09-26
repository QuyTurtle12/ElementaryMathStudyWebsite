﻿using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAppAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IAppUserServices _userServices;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public AuthController(IAppAuthService authService, ITokenService tokenService, IAppUserServices userServices, IRoleService roleService, IMapper mapper)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _tokenService = tokenService;
            _userServices = userServices;
            _roleService = roleService;
            _mapper = mapper;
        }

        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        /// <param name="loginDto">The login request data.</param>
        /// <returns>A JWT token if successful.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Authenticate and generate token
                string token = await _authService.LoginAsync(loginDto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
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
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDto">The registration request data.</param>
        /// <returns>A message indicating the result of the registration attempt.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _authService.RegisterAsync(registerDto);
                var response = BaseResponse<String>.OkResponse("Registration successful. Please check your email for verification.");

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // Conflict for cases like existing user or invalid role
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
        /// Registers a new student.
        /// </summary>
        /// <param name="registerDto">The registration request data.</param>
        /// <returns>A message indicating the result of the registration attempt.</returns>
        [HttpPost("student-register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Authorize(Policy = "Parent")]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "Registing children for parent"
            )]
        public async Task<IActionResult> StudentRegisterAsync([FromBody] StudentRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Extract user ID from the token
            var userId = _tokenService.GetUserIdFromTokenHeader(token);

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }
            var user = await _userServices.GetUserByIdAsync(userId.ToString());

            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                throw new BaseException.NotFoundException("not_found", "User or Email not found");
            }

            try
            {
                await _authService.StudentRegisterAsync(registerDto, user.Email, user.Id);
                var response = BaseResponse<String>.OkResponse("Registration successful. Please check your email for verification.");

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // Conflict for cases like existing user or invalid role
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
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle specific BadRequestException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmailAsync([FromQuery] string token)
        {
            try
            {
                await _authService.VerifyEmailAsync(token);
                var response = BaseResponse<String>.OkResponse("Email verified successfully.");

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
        [Route("role-pagination")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Get page with all roles "
            )]
        public async Task<IActionResult> GetAllRoles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedRoles = await _roleService.GetAllRolesAsync(pageNumber, pageSize);

                // Map users to UserResponseDto
                var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(paginatedRoles.Items);

                // Create a new paginated list of UserResponseDto
                var paginatedRoleDtos = new BasePaginatedList<RoleDto>(
                    roleDtos.ToList(),
                    paginatedRoles.TotalItems,
                    paginatedRoles.CurrentPage,
                    paginatedRoles.PageSize
                );

                var response = BaseResponse<BasePaginatedList<RoleDto>>.OkResponse(paginatedRoleDtos);

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

        [HttpPost]
        [Route("role/create")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Creating new role"
            )]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            if (createRoleDto == null)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Role data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var role = await _roleService.CreateRoleAsync(createRoleDto);
                var roleResponseDto = _mapper.Map<RoleDto>(role);

                var response = BaseResponse<RoleDto>.OkResponse(roleResponseDto);

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
