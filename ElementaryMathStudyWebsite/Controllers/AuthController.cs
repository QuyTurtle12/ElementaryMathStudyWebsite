using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
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
        /// Logs in a role and returns a JWT token.
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

            // Authenticate and generate token
            string token = await _authService.LoginAsync(loginDto);
            return Ok(new { Token = token });

        }

        /// <summary>
        /// Registers a new role.
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

            await _authService.RegisterAsync(registerDto);
            var response = BaseResponse<string>.OkResponse("Registration successful. Please check your email for verification.");

            return Ok(response);
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId) || userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var user = await _userServices.GetUserByIdAsync(userId.ToString());

            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                throw new BaseException.NotFoundException("not_found", "User or Email not found");
            }

            await _authService.StudentRegisterAsync(registerDto, user.Email, user.Id);
            var response = BaseResponse<String>.OkResponse("Registration successful. Please check your email for verification.");

            return Ok(response);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmailAsync([FromQuery] string token)
        {
            await _authService.VerifyEmailAsync(token);
            var response = BaseResponse<String>.OkResponse("Email verified successfully.");

            return Ok(response);
        }

        [HttpGet]
        [Route("role")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Get page with all roles "
            )]
        public async Task<IActionResult> GetAllRoles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
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

        [HttpPost]
        [Route("role/create")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Creating new role"
            )]
        public async Task<IActionResult> CreateRole([FromBody] RequestRole createRoleDto)
        {
            if (createRoleDto == null)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Role data is required.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var role = await _roleService.CreateRoleAsync(createRoleDto);
            var roleResponseDto = _mapper.Map<RoleDto>(role);

            var response = BaseResponse<RoleDto>.OkResponse(roleResponseDto);

            return Ok(response);

        }

        [HttpPut]
        [Route("role/update/{roleId}")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Updating a Role"
            )]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] RequestRole roleDto)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new BaseException.BadRequestException("invalid_argument", "Role ID is required.");
            }

            if (roleDto == null)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Role data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            var role = await _roleService.UpdateRoleAsync(roleId, roleDto);
            var roleResponseDto = _mapper.Map<RoleDto>(role);

            var response = BaseResponse<RoleDto>.OkResponse(roleResponseDto);

            return Ok(response);

        }

        [HttpGet]
        [Route("role/{roleId}")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Get a role by id"
            )]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new BaseException.BadRequestException("invalid_argument", "Role ID is required.");
            }

            var role = await _roleService.GetRoleByIdAsync(roleId);


            var roleResponseDto = _mapper.Map<RoleDto>(role);

            var response = BaseResponse<RoleDto>.OkResponse(roleResponseDto);

            return Ok(response);

        }

        [HttpDelete]
        [Route("role/delete/{roleId}")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Deleting a Role"
            )]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new BaseException.BadRequestException("invalid_argument", "Role ID is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            var role = await _roleService.DeleteRoleAsync(roleId);

            var response = BaseResponse<string>.OkResponse("Delete successfully");

            return Ok(response);

        }

        /// <summary>
        /// Sends a password reset link to the user's email.
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="userName">User's username</param>
        /// <returns>ActionResult indicating the result of the operation.</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _authService.ForgotPasswordAsync(request.Email, request.UserName);
            var response = BaseResponse<string>.OkResponse("Password reset link has been sent to your email.");

            return Ok(response);
        }

        /// <summary>
        /// Resets the user's password using a token.
        /// </summary>
        /// <param name="resetPasswordDto">The reset password DTO containing the token and new password.</param>
        /// <returns>ActionResult indicating the result of the operation.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _authService.ResetPasswordAsync(resetPasswordDto.Token, resetPasswordDto.NewPassword);
            var response = BaseResponse<string>.OkResponse("Password has been successfully reset.");

            return Ok(response);
        }

        [HttpGet("verify-reset-password-token")]
        public async Task<IActionResult> VerifyResetPasswordEmailAsync([FromQuery] string token)
        {

            await _authService.VerifyResetPasswordTokenAsync(token);
            var response = BaseResponse<String>.OkResponse(token);

            return Ok(response);
        }
    }
}
