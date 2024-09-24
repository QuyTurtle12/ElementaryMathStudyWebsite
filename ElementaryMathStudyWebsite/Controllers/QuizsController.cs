﻿using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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
                var quizzes = await _quizService.GetAllQuizzesAsync();
                if (quizzes == null) return NotFound(new { message = "Quizzes not found" });
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
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin & Manager", Description = "Retrieve a quiz by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<QuizMainViewDto>>> GetQuizById(string id)
        {
            try
            {
                var quiz = await _quizService.GetQuizByQuizIdAsync(id);
                if (quiz == null) return NotFound(new { message = "Quiz not found" });
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
                var quizzes = await _quizService.GetQuizzesByChapterOrTopicIdAsync(chapterId, null);
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
                var quizzes = await _quizService.GetQuizzesByChapterOrTopicIdAsync(null, topicId);
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
                var quizzes = await _quizService.SearchQuizzesByNameAsync(quizName);
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
                var quizzes = await _quizService.GetQuizzesAsync(pageNumber, pageSize);
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
                var createdQuiz = await _quizService.AddQuizAsync(dto);
                return BaseResponse<QuizMainViewDto>.OkResponse(createdQuiz, "Quiz created successfully.");
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
        public async Task<ActionResult<BaseResponse<QuizUpdateDto>>> UpdateQuizAsync([FromBody] QuizUpdateDto dto)
        {
            try
            {
                // Update the quiz and get the updated data
                var updatedQuizDto = await _quizService.UpdateQuizAsync(dto);
                return BaseResponse<QuizUpdateDto>.OkResponse(updatedQuizDto, "Quiz updated successfully.");
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
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete an existing quiz.", Description = "Deletes a quiz by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<DeleteQuizDto>>> DeleteQuizAsync(string id)
        {
            try
            {
                // Delete the quiz and get the deleted quiz data
                var deletedQuizDto = await _quizService.DeleteQuizAsync(id);
                return BaseResponse<DeleteQuizDto>.OkResponse(deletedQuizDto, "Quiz deleted successfully.");
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
