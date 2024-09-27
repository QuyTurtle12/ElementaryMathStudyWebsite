using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Services.Service;
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
            var response = BaseResponse<BasePaginatedList<UserAnswerWithDetailsDTO>>.OkResponse(result);
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
        public async Task<ActionResult<BaseResponse<UserAnswer>>> CreateUserAnswers([FromBody] UserAnswerCreateDTO userAnswerCreateDTO)
        {
            try
            {
                if (userAnswerCreateDTO == null || !userAnswerCreateDTO.UserAnswerList.Any())
                {
                    throw new BaseException.BadRequestException("input_error", "Invalid input. The user answer list is empty or null.");
                }

                var createdUserAnswers = await _userAnswerService.CreateUserAnswersAsync(userAnswerCreateDTO);

                if (!createdUserAnswers.IsAddedResult)
                {
                    throw new BaseException.CoreException("error", "Failed to add student result");
                }

                if (createdUserAnswers.IsPassedTheQuiz)
                {
                    // Return only a success message
                    var passedQuizResponse = BaseResponse<UserAnswer>.OkResponse("Congratulations, you passed the quiz.Keep it up!");
                    return passedQuizResponse;
                }

                // Return only a failure message
                var failedQuizResponse = BaseResponse<UserAnswer>.OkResponse("You worked hard, but hard is not enough, try again next time");
                return failedQuizResponse;

            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                return NotFound(new { errorCode = notFoundEx.ErrorDetail.ErrorCode, errorMessage = notFoundEx.ErrorDetail.ErrorMessage });
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
