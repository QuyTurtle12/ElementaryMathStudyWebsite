using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null)
            {
                return BadRequest("User data is required.");
            }

            // Validate input data here if needed
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.CreateUserAsync(createUserDto);

                // Return 201 Created with the location of the newly created user
                return CreatedAtAction(nameof(CreateUser), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                // Handle specific validation exceptions
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // e.g., _logger.LogError(ex, "An error occurred while creating the user.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
