using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly IAppResultService _resultService;

        // Constructor
        public ResultsController(IAppResultService resultService)
        {
            _resultService = resultService;
        }

        // POST: api/results
        // Add Result
        [Authorize(Policy = "Student")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authorization: Student",
            Description = "Save student quiz's result"
            )]
        public async Task<ActionResult<BaseResponse<Result>>>AddResult(ResultCreateDto studentResult)
        {
            try
            {
                ResultProgressDto result = await _resultService.AddStudentResultAsync(studentResult);

                if (!result.IsAddedResult)
                {
                    throw new BaseException.CoreException("error", "Failed to add student result");
                }

                if (result.IsPassedTheQuiz)
                {
                    // Return only a success message
                    var passedQuizResponse = BaseResponse<Result>.OkResponse("Congratulations, you passed the quiz.Keep it up!");
                    return passedQuizResponse;
                }

                // Return only a failure message
                var failedQuizResponse = BaseResponse<Result>.OkResponse("You worked hard, but hard is not enough, try again next time");
                return failedQuizResponse;
  
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

        // GET: api/results
        // Get student result of specific quiz
        [Authorize(Policy = "Student")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: Student",
            Description = "Get a list of student result of specific quiz"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ResultViewDto>>>> GetQuizResult([Required] string quizId , int pageSize = -1, int pageNumber = -1)
        {
            try
            {
                BasePaginatedList<ResultViewDto> results = await _resultService.GetStudentResultListAsync(quizId, pageNumber, pageSize);

                if (!results.Items.Any())
                {
                    throw new BaseException.BadRequestException("empty_list","This student hasn't done any test in this quiz yet");
                }

                var response = BaseResponse<BasePaginatedList<ResultViewDto>>.OkResponse(results);
                return response;
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

        // GET: api/results/parent
        // Get student result of specific quiz
        [Authorize(Policy = "Parent")]
        [HttpGet]
        [Route("parent")]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "Get a list of quiz result of specific child"
            )]
        public async Task<ActionResult<BaseResponse<ResultParentViewDto>>> GetChildResult([Required] string studentId)
        {
            try
            {
                ResultParentViewDto result = await _resultService.GetChildrenLatestResultAsync(studentId);

                var response = BaseResponse<ResultParentViewDto>.OkResponse(result);

                return response;
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
