using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAnswersController(IAppUserAnswerServices userAnswerService) : ControllerBase
    {
        private readonly IAppUserAnswerServices _userAnswerService = userAnswerService ?? throw new ArgumentNullException(nameof(userAnswerService));

        // GET: api/UserAnswers
        //[Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Get all the user answers by paging, pageSize = -1 to get all"
        )]
        [HttpGet]
        public async Task<IActionResult> GetAllUserAnswers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userAnswerService.GetAllUserAnswersAsync(pageNumber, pageSize);
            var response = BaseResponse<BasePaginatedList<object>>.OkResponse(result);
            return Ok(response);
        }

        // GET: api/UserAnswers/{id}
        //[Authorize(Policy = "Admin-Content")]
        //[SwaggerOperation(
        //    Summary = "Authorization: Admin, Content Manager",
        //    Description = "Get the user answer by id"
        //)]
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetUserAnswerById(string id)
        //{
        //    try
        //    {
        //        var userAnswer = await _userAnswerService.GetUserAnswerByIdAsync(id);
        //        var response = BaseResponse<object>.OkResponse(userAnswer);
        //        return Ok(response);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}

        [Authorize(Policy = "Student")]
        [SwaggerOperation(
            Summary = "Authorization: Student",
            Description = "When user answers create an instance of it"
        )]
        [HttpPost]
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateUserAnswers([FromBody] UserAnswerCreateDTO userAnswerCreateDTO)
        {
            if (userAnswerCreateDTO == null || !userAnswerCreateDTO.UserAnswerList.Any())
            {
                return BadRequest(new BaseException.BadRequestException("input_error", "Invalid input. The user answer list is empty or null."));
            }

            try
            {
                var createdUserAnswers = await _userAnswerService.CreateUserAnswersAsync(userAnswerCreateDTO);
                var response = BaseResponse<List<UserAnswerWithDetailsDTO>>.OkResponse(createdUserAnswers);
                return Ok(response);
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                return NotFound(new { errorCode = notFoundEx.ErrorDetail.ErrorCode, errorMessage = notFoundEx.ErrorDetail.ErrorMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while creating the user answers: {ex.Message}" });
            }
        }


        // PUT: api/UserAnswers/{id}
        //[Authorize(Policy = "Admin-Content")]
        //[SwaggerOperation(
        //    Summary = "Authorization: Admin, Content Manager",
        //    Description = "Update user answer"
        //)]
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateUserAnswer(string id, [FromBody] UserAnswerDTO userAnswerDTO)
        //{
        //    if (userAnswerDTO == null)
        //    {
        //        return BadRequest(new BaseException.BadRequestException("input_error", "Invalid input."));
        //    }

        //    try
        //    {
        //        var userAnswer = await _userAnswerService.UpdateUserAnswerAsync(id, userAnswerDTO);
        //        var response = BaseResponse<object>.OkResponse(userAnswer);
        //        return Ok(response);
        //    }
        //    catch (BaseException.NotFoundException notFoundEx)
        //    {
        //        return NotFound(new { errorCode = notFoundEx.ErrorDetail.ErrorCode, errorMessage = notFoundEx.ErrorDetail.ErrorMessage });
        //    }
        //}

        // GET: api/QuizAnswers/quiz/{quizId}
        [Authorize(Policy = "Student")]
        [SwaggerOperation(
            Summary = "Authorization: Student",
            Description = "Get all answers from the current student for the given quiz"
        )]
        [HttpGet("quiz/{quizId}")]
        public async Task<IActionResult> GetUserAnswersByQuizId(string quizId)
        {
            try
            {
                var userAnswers = await _userAnswerService.GetUserAnswersByQuizIdAsync(quizId);
                var response = BaseResponse<BasePaginatedList<UserAnswerWithDetailsDTO>>.OkResponse(userAnswers);
                return Ok(response);
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                return NotFound(new { errorCode = notFoundEx.ErrorDetail.ErrorCode, errorMessage = notFoundEx.ErrorDetail.ErrorMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
