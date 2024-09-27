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
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Retrieve all quizzes. Admin access required.")]
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
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Retrieve a quiz by its unique identifier.")]
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
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Retrieve all quizzes belonging to a specific chapter.")]
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
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Retrieve all quizzes belonging to a specific topic.")]
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
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Search for quizzes by name.")]
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
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("paged")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Retrieve quizzes with pagination.")]
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
        [Authorize(Policy = "Admin-Content")]
        [HttpPost]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Creates a new quiz and returns the created quiz.")]
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
        [Authorize(Policy = "Admin-Content")]
        [HttpPut]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Updates an existing quiz based on the provided data.")]
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
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Deletes a quiz by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<QuizDeleteDto>>> DeleteQuizAsync(string id)
        {
            try
            {
                // Delete the quiz and get the deleted quiz data
                var deletedQuizDto = await _quizService.DeleteQuizAsync(id)
                    ?? throw new BaseException.NotFoundException("not_found", "quiz not found.");
                return BaseResponse<QuizDeleteDto>.OkResponse("Quiz deleted successfully.");
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
    }
}
