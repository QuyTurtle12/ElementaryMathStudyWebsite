using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ElementaryMathStudyWebsite.Core.Base;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace ElementaryMathStudyWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IAppQuizServices _quizService;

        public QuizController(IAppQuizServices quizService)
        {
            _quizService = quizService;
        }

        // GET: api/quiz/all
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Authorization: Admin", Description = "Retrieve all quizzes. Admin access required.")]
        public async Task<ActionResult<BaseResponse<List<QuizMainViewDto>>>> GetAllQuizzes()
        {
            try
            {
                var quizzes = await _quizService.GetAllQuizzesAsync()
                    ?? throw new BaseException.NotFoundException("not_found", "quizzes not found.");
                return BaseResponse<List<QuizMainViewDto>>.OkResponse(quizzes);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // GET: api/quiz/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin & Manager", Description = "Retrieve a quiz by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<QuizMainViewDto>>> GetQuizById(string id)
        {
            try
            {
                var quiz = await _quizService.GetQuizByQuizIdAsync(id)
                    ?? throw new BaseException.NotFoundException("not_found", "quiz not found.");
                return BaseResponse<QuizMainViewDto>.OkResponse(quiz);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // GET: api/quiz/chapter/{chapterId}
        [HttpGet("chapter/{chapterId}")]
        [SwaggerOperation(Summary = "Authorization: Admin", Description = "Retrieve all quizzes belonging to a specific chapter.")]
        public async Task<ActionResult<BaseResponse<List<QuizViewDto>>>> GetQuizzesByChapterId(string chapterId)
        {
            try
            {
                var quizzes = await _quizService.GetQuizzesByChapterOrTopicIdAsync(chapterId, null)
                    ?? throw new BaseException.NotFoundException("not_found", "quizzes not found.");
                return BaseResponse<List<QuizViewDto>>.OkResponse(quizzes);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // GET: api/quiz/topic/{topicId}
        [HttpGet("topic/{topicId}")]
        [SwaggerOperation(Summary = "Authorization: Admin", Description = "Retrieve all quizzes belonging to a specific topic.")]
        public async Task<ActionResult<BaseResponse<List<QuizViewDto>>>> GetQuizzesByTopicId(string topicId)
        {
            try
            {
                var quizzes = await _quizService.GetQuizzesByChapterOrTopicIdAsync(null, topicId)
                    ?? throw new BaseException.NotFoundException("not_found", "quizzes not found.");
                return BaseResponse<List<QuizViewDto>>.OkResponse(quizzes);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // GET: api/quiz/search
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Authorization: Admin & Manager", Description = "Search for quizzes by name.")]
        public async Task<ActionResult<BaseResponse<List<QuizViewDto>>>> SearchQuizzesByName([FromQuery, Required] string quizName)
        {
            try
            {
                var quizzes = await _quizService.SearchQuizzesByNameAsync(quizName)
                    ?? throw new BaseException.NotFoundException("not_found", "quiz not found.");
                return BaseResponse<List<QuizViewDto>>.OkResponse(quizzes);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // GET: api/quiz/paged
        [HttpGet("paged")]
        [SwaggerOperation(Summary = "Authorization: Admin & Manager", Description = "Retrieve quizzes with pagination.")]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<QuizMainViewDto>>>> GetQuizzesPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var quizzes = await _quizService.GetQuizzesAsync(pageNumber, pageSize)
                    ?? throw new BaseException.NotFoundException("not_found", "quizzes not found.");
                return BaseResponse<BasePaginatedList<QuizMainViewDto>>.OkResponse(quizzes);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // POST: api/quiz
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new quiz.", Description = "Creates a new quiz and returns the created quiz.")]
        public async Task<ActionResult<BaseResponse<QuizMainViewDto>>> AddQuizAsync([FromBody] QuizCreateDto dto)
        {
            try
            {
                var createdQuiz = await _quizService.AddQuizAsync(dto)
                    ?? throw new BaseException.NotFoundException("not_found", "quizzes not found.");
                return BaseResponse<QuizMainViewDto>.OkResponse("Quiz created successfully.");
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // PUT: api/quiz
        [HttpPut]
        [SwaggerOperation(Summary = "Update an existing quiz.", Description = "Updates an existing quiz based on the provided data.")]
        public async Task<ActionResult<BaseResponse<QuizMainViewDto>>> UpdateQuizAsync([Required] string id, [FromBody] QuizUpdateDto dto)
        {
            try
            {
                // Update the quiz and get the updated data
                var updatedQuizDto = await _quizService.UpdateQuizAsync(id, dto)
                    ?? throw new BaseException.NotFoundException("not_found", "quiz not found.");
                return BaseResponse<QuizMainViewDto>.OkResponse("Quiz updated successfully.");
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle core exceptions
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle bad request exceptions
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // DELETE: api/quiz/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a quiz.", Description = "Marks a quiz as deleted.")]
        public async Task<IActionResult> DeleteQuizAsync(string id)
        {
            try
            {
                var result = await _quizService.DeleteQuizAsync(id);

                if (result)
                {
                    var successResponse = BaseResponse<string>.OkResponse("Delete successfully");
                    return Ok(successResponse);

                }
                var failedResponse = BaseResponse<string>.OkResponse("Delete unsuccessfully");

                return Ok(failedResponse);
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
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }
        }
    }
}
