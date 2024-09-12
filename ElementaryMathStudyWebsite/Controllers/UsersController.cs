using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        [Route("create")]
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
                var user = await _userService.CreateUserAsync(createUserDto);
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

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedUsers = await _userService.GetAllUsersAsync(pageNumber, pageSize);
                return Ok(paginatedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
