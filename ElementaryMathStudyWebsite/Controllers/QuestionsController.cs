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
    public class QuestionController : ControllerBase
    {
        private readonly IAppQuestionServices _questionService;

        public QuestionController(IAppQuestionServices questionService)
        {
            _questionService = questionService;
        }

        // GET: api/question/all
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Authorization: Admin, Content Manager", Description = "Retrieve all questions.")]
        public async Task<ActionResult<BaseResponse<List<QuestionMainViewDto>>>> GetAllQuestions()
        {
            try
            {
                // Fetch all questions from the service
                var questions = await _questionService.GetAllQuestionsMainViewDtoAsync()
                      ?? throw new BaseException.NotFoundException("not_found", "questions not found.");

                // Return success response with the fetched questions
                return BaseResponse<List<QuestionMainViewDto>>.OkResponse(questions);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle and return CoreException with its status code
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle and return BadRequestException with detailed error code and message
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }

        // GET: api/question/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Retrieve a specific question by its ID.")]
        public async Task<ActionResult<BaseResponse<QuestionMainViewDto>>> GetQuestionById(string id)
        {
            try
            {
                // Fetch a question by its ID
                var question = await _questionService.GetQuestionByIdAsync(id)
                      ?? throw new BaseException.NotFoundException("not_found", "questions not found.");

                // Return success response with the question details
                return BaseResponse<QuestionMainViewDto>.OkResponse(question);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle and return CoreException with its status code
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle and return BadRequestException with detailed error code and message
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }


        // GET: api/question/search
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search questions by Content, Authorization: Manager & Admin", Description = "Search for questions where the Content contains the specified string.")]
        public async Task<ActionResult<BaseResponse<List<QuestionViewDto>>>> SearchQuestions([FromQuery] string Content)
        {
            try
            {
                // Search questions by the provided Content string
                var questions = await _questionService.SearchQuestionsByContextAsync(Content)
                    ?? throw new BaseException.NotFoundException("not_found", "questions not found.");

                // Return success response with the search results
                return BaseResponse<List<QuestionViewDto>>.OkResponse(questions);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle and return CoreException with its status code
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle and return BadRequestException with detailed error code and message
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }

        // GET: api/question/quiz/{quizId}
        [HttpGet("quiz/{quizId}")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Retrieve all questions for a specific quiz.")]
        public async Task<ActionResult<BaseResponse<List<QuestionViewDto>>>> GetQuestionsByQuizId(string quizId)
        {
            try
            {
                // Fetch all questions that belong to the specified quiz
                var questions = await _questionService.GetQuestionsByQuizIdAsync(quizId)
                    ?? throw new BaseException.NotFoundException("not_found", "questions not found.");

                // Return success response with the list of questions
                return BaseResponse<List<QuestionViewDto>>.OkResponse(questions);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle and return CoreException with its status code
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle and return BadRequestException with detailed error code and message
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }

        // GET: api/question
        [Authorize(Policy = "Admin-Content")]
        [HttpGet]
        [SwaggerOperation(Summary = "Authorization: Admin, Content Manager", Description = "Retrieve all questions with pagination.")]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<QuestionMainViewDto>>>> GetQuestions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Fetch paginated list of questions
                var paginatedQuestions = await _questionService.GetQuestionsAsync(pageNumber, pageSize)
                    ?? throw new BaseException.NotFoundException("not_found", "questions not found.");

                // Return success response with paginated data
                return BaseResponse<BasePaginatedList<QuestionMainViewDto>>.OkResponse(paginatedQuestions);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle and return CoreException with its status code
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle and return BadRequestException with detailed error code and message
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }

        // POST: api/question
        [Authorize(Policy = "Admin-Content")]
        [HttpPost]
        [SwaggerOperation(Summary = "Authorization: Admin, Content Manager", Description = "Creates a new question and returns the created question.")]
        public async Task<ActionResult<BaseResponse<QuestionMainViewDto>>> AddQuestionAsync([FromBody] QuestionCreateDto dto)
        {
            try
            {
                var createdQuestion = await _questionService.AddQuestionAsync(dto)
                    ?? throw new BaseException.NotFoundException("not_found", "questions not found.");
                return BaseResponse<QuestionMainViewDto>.OkResponse("Question created successfully.");
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

        // PUT: api/question/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin, Content Manager", Description = "Updates an existing question based on the provided data.")]
        public async Task<ActionResult<BaseResponse<QuestionMainViewDto>>> UpdateQuestionAsync([Required] string id, [FromBody] QuestionUpdateDto dto)
        {
            try
            {
                // Update the question and get the updated data
                var updatedQuestionDto = await _questionService.UpdateQuestionAsync(id, dto)
                    ?? throw new BaseException.NotFoundException("not_found", "question not found.");
                return BaseResponse<QuestionMainViewDto>.OkResponse("Question updated successfully.");
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


        // DELETE: api/question/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Delete a question")]
        public async Task<IActionResult> DeleteQuestion([Required] string id)
        {
            try
            {
                var result = await _questionService.DeleteQuestion(id);

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