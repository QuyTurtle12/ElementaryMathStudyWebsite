using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IAppQuestionServices _questionService;

        public QuestionController(IAppQuestionServices questionService)
        {
            _questionService = questionService;
        }

        // GET: api/question/all
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all questions", Description = "Retrieve all questions.")]
        public async Task<ActionResult<BaseResponse<List<QuestionMainViewDto>>>> GetAllQuestions()
        {
            try
            {
                var questions = await _questionService.GetAllQuestionsMainViewDtoAsync();
                return BaseResponse<List<QuestionMainViewDto>>.OkResponse(questions);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // GET: api/question/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get question by ID", Description = "Retrieve a specific question by its ID.")]
        public async Task<ActionResult<BaseResponse<QuestionMainViewDto>>> GetQuestionById(string id)
        {
            try
            {
                var question = await _questionService.GetQuestionByIdAsync(id);
                if (question == null)
                {
                    return NotFound(new { message = "Question not found." });
                }
                return BaseResponse<QuestionMainViewDto>.OkResponse(question);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // GET: api/question/search
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search questions by context", Description = "Search for questions where the context contains the specified string.")]
        public async Task<ActionResult<BaseResponse<List<QuestionViewDto>>>> SearchQuestions([FromQuery] string context)
        {
            try
            {
                var questions = await _questionService.SearchQuestionsByContextAsync(context);
                return BaseResponse<List<QuestionViewDto>>.OkResponse(questions);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/question/quiz/{quizId}
        [HttpGet("quiz/{quizId}")]
        [SwaggerOperation(Summary = "Get questions by quiz Id", Description = "Retrieve all questions for a specific quiz.")]
        public async Task<ActionResult<BaseResponse<List<QuestionViewDto>>>> GetQuestionsByQuizId(string quizId)
        {
            try
            {
                var questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
                return BaseResponse<List<QuestionViewDto>>.OkResponse(questions);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/question
        [HttpGet]
        [SwaggerOperation(Summary = "Get questions with pagination", Description = "Retrieve all questions with pagination.")]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<QuestionMainViewDto>>>> GetQuestions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Call the service method to get paginated questions
                var paginatedQuestions = await _questionService.GetQuestionsAsync(pageNumber, pageSize);

                // Return the successful response with the paginated data
                return BaseResponse<BasePaginatedList<QuestionMainViewDto>>.OkResponse(paginatedQuestions);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle CoreException with custom status code and message
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle BadRequestException with specific error details
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                // Handle generic exception and return status code 500 with the exception message
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}