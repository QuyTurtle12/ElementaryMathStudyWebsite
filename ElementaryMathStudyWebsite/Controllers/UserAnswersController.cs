using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAnswersController(IAppUserAnswerServices userAnswerService) : ControllerBase
    {
        private readonly IAppUserAnswerServices _userAnswerService = userAnswerService ?? throw new ArgumentNullException(nameof(userAnswerService));

        // GET: api/UserAnswers
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Get all the user answers by paging, pageSize = -1 to get all"
        )]
        [HttpGet]
        public async Task<IActionResult> GetAllUserAnswers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userAnswerService.GetAllUserAnswersAsync(pageNumber, pageSize);
            return Ok(result);
        }

        // GET: api/UserAnswers/{id}
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Get the user answer by id"
        )]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAnswerById(string id)
        {
            try
            {
                var userAnswer = await _userAnswerService.GetUserAnswerByIdAsync(id);
                return Ok(userAnswer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "When user answers create an instance of it"
        )]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateUserAnswer([FromBody] UserAnswerDTO userAnswerDTO)
        {
            if (userAnswerDTO == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }

            try
            {
                var createdUserAnswer = await _userAnswerService.CreateUserAnswerAsync(userAnswerDTO);
                return Ok(createdUserAnswer); // Return the created UserAnswerDTO if successful
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while creating the user answer." });
            }
        }

        // PUT: api/UserAnswers/{id}
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Update user answer"
        )]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAnswer(string id, [FromBody] UserAnswerDTO userAnswerDTO)
        {
            if (userAnswerDTO == null)
            {
                return BadRequest(new { message = "Invalid user answer data." });
            }

            try
            {
                var userAnswer = await _userAnswerService.UpdateUserAnswerAsync(id, userAnswerDTO);
                return Ok(userAnswer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/UserAnswers/{id}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUserAnswer(string id)
        //{
        //    await _userAnswerService.DeleteUserAnswerAsync(id);
        //    return NoContent();
        //}
    }
}
