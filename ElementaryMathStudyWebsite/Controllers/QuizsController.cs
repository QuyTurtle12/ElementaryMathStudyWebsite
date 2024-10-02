using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ElementaryMathStudyWebsite.Core.Base;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

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
        //[Authorize(Policy = "Admin-Content")]
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Retrieve all quizzes. Admin access required.")]
        public async Task<ActionResult<BaseResponse<List<QuizMainViewDto>>>> GetAllQuizzes()
        {
            List<QuizMainViewDto> quizzes = await _quizService.GetAllQuizzesAsync();
            return BaseResponse<List<QuizMainViewDto>>.OkResponse(quizzes);
        }

        // GET: api/quiz/{id}
        //[Authorize(Policy = "Admin-Content")]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Retrieve a quiz by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<QuizMainViewDto>>> GetQuizById(string id)
        {
            QuizMainViewDto quiz = await _quizService.GetQuizByQuizIdAsync(id);
            return BaseResponse<QuizMainViewDto>.OkResponse(quiz);
        }

        // GET: api/quiz/chapter/{chapterId}
        [HttpGet("chapter/{chapterId}")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Retrieve all quizzes belonging to a specific chapter.")]
        public async Task<ActionResult<BaseResponse<List<QuizViewDto>>>> GetQuizzesByChapterId(string chapterId)
        {
            List<QuizViewDto> quizzes = await _quizService.GetQuizzesByChapterOrTopicIdAsync(chapterId, null);
            return BaseResponse<List<QuizViewDto>>.OkResponse(quizzes);
        }

        // GET: api/quiz/topic/{topicId}
        [HttpGet("topic/{topicId}")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Retrieve all quizzes belonging to a specific topic.")]
        public async Task<ActionResult<BaseResponse<List<QuizViewDto>>>> GetQuizzesByTopicId(string topicId)
        {
            List<QuizViewDto> quizzes = await _quizService.GetQuizzesByChapterOrTopicIdAsync(null, topicId);
            return BaseResponse<List<QuizViewDto>>.OkResponse(quizzes);
        }

        // GET: api/quiz/search
        //[Authorize(Policy = "Admin-Content")]
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Search for quizzes by name.")]
        public async Task<ActionResult<BaseResponse<List<QuizViewDto>>>> SearchQuizzesByName([FromQuery, Required] string quizName)
        {
            List<QuizViewDto> quizzes = await _quizService.SearchQuizzesByNameAsync(quizName);
            return BaseResponse<List<QuizViewDto>>.OkResponse(quizzes);
        }

        // GET: api/quiz/paged
        //[Authorize(Policy = "Admin-Content")]
        // for all user
        [HttpGet("paged")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Retrieve quizzes with pagination.")]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<QuizViewDto>>>> GetQuizzesPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            BasePaginatedList<QuizViewDto> quizzes = await _quizService.GetQuizzesAsync(pageNumber, pageSize);
            return BaseResponse<BasePaginatedList<QuizViewDto>>.OkResponse(quizzes);
        }

        // POST: api/quiz
        //[Authorize(Policy = "Admin-Content")]
        [HttpPost]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Creates a new quiz and returns the created quiz.")]
        public async Task<ActionResult<BaseResponse<QuizMainViewDto>>> AddQuizAsync([FromBody] QuizCreateDto dto)
        {
           QuizMainViewDto createdQuiz = await _quizService.AddQuizAsync(dto);
            return BaseResponse<QuizMainViewDto>.OkResponse(createdQuiz, "Quiz created successfully");
        }

        // PUT: api/quiz
        // [Authorize(Policy = "Admin-Content")]
        [HttpPut]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Updates an existing quiz based on the provided data.")]
        public async Task<ActionResult<BaseResponse<QuizMainViewDto>>> UpdateQuizAsync([Required] string id, [FromBody] QuizUpdateDto dto)
        {
            // Update the quiz and get the updated data
            QuizMainViewDto updatedQuizDto = await _quizService.UpdateQuizAsync(id, dto);
            return BaseResponse<QuizMainViewDto>.OkResponse(updatedQuizDto, "Quiz updated successfully.");
        }

        // DELETE: api/quiz/{id}
        //[Authorize(Policy = "Admin-Content")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a quiz.", Description = "Marks a quiz as deleted.")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteQuizAsync(string id)
        {
            // Call the service method and get the result wrapped in BaseResponse
            BaseResponse<string> response = await _quizService.DeleteQuizAsync(id);
            return Ok(response);
        }
    }
}
