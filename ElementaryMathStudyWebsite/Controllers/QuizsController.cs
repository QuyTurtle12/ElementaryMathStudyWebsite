using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ElementaryMathStudyWebsite.Core.Base;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IAppQuizServices _quizService;
        private readonly IAppUserServices _userService;

        public QuizController(IAppQuizServices quizService, IAppUserServices userService)
        {
            _quizService = quizService;
            _userService = userService;
        }

        // GET: api/quiz/all
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Retrieve all quizzes. Admin access required.")]
        public async Task<ActionResult<BaseResponse<List<QuizMainViewDto>>>> GetAllQuizzes()
        {
            List<QuizMainViewDto> quizzes = await _quizService.GetAllQuizzesAsync();
            return BaseResponse<List<QuizMainViewDto>>.OkResponse(quizzes);
        }

        // GET: api/quiz/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Retrieve a quiz by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<QuizMainViewDto>>> GetQuizById(string id)
        {
            QuizMainViewDto quiz = await _quizService.GetQuizByQuizIdAsync(id);
            return BaseResponse<QuizMainViewDto>.OkResponse(quiz);
        }

        // POST: api/quiz
        [Authorize(Policy = "Admin-Content")]
        [HttpPost]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Creates a new quiz.")]
        public async Task<ActionResult<BaseResponse<QuizMainViewDto>>> AddQuizAsync([FromBody] QuizCreateDto dto)
        {
            User currentUser = await _userService.GetCurrentUserAsync();
            QuizMainViewDto? createdQuiz = await _quizService.AddQuizAsync(dto, currentUser);
            return BaseResponse<QuizMainViewDto>.OkResponse(createdQuiz, "Quiz created successfully");
        }

        // PUT: api/quiz
        [Authorize(Policy = "Admin-Content")]
        [HttpPut]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Updates an existing quiz based on the provided data.")]
        public async Task<ActionResult<BaseResponse<QuizMainViewDto>>> UpdateQuizAsync([Required] string id, [FromBody] QuizUpdateDto dto)
        {
            User currentUser = await _userService.GetCurrentUserAsync();
            QuizMainViewDto? updatedQuizDto = await _quizService.UpdateQuizAsync(id, dto, currentUser);
            return BaseResponse<QuizMainViewDto>.OkResponse(updatedQuizDto, "Quiz updated successfully.");
        }

        // DELETE: api/quiz/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Delete a quiz and marks a quiz as deleted.")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteQuizAsync(string id)
        {
            // Call the service method and get the result wrapped in BaseResponse
            BaseResponse<string> response = await _quizService.DeleteQuizAsync(id);
            return Ok(response);
        }
        
        //=======================================================================================================================================================

        // GET: api/quiz/chapter/{chapterId}
        [HttpGet("chapter/{chapterId}")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Retrieve all quizzes belonging to a specific chapter.")]
        public async Task<ActionResult<BaseResponse<QuizViewDto>>> GetQuizzesByChapterId(string chapterId)
        {
            QuizViewDto? quizzes = await _quizService.GetQuizByChapterOrTopicIdAsync(chapterId, null);
            return BaseResponse<QuizViewDto>.OkResponse(quizzes);
        }

        // GET: api/quiz/topic/{topicId}
        [HttpGet("topic/{topicId}")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Retrieve all quizzes belonging to a specific topic.")]
        public async Task<ActionResult<BaseResponse<QuizViewDto>>> GetQuizzesByTopicId(string topicId)
        {
            QuizViewDto? quizzes = await _quizService.GetQuizByChapterOrTopicIdAsync(null, topicId);
            return BaseResponse<QuizViewDto>.OkResponse(quizzes);
        }
            
        // GET: api/quiz/search
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Search for quizzes by name.")]
        public async Task<ActionResult<BaseResponse<List<QuizViewDto>>>> SearchQuizzesByName([FromQuery, Required] string quizName)
        {
            List<QuizViewDto> quizzes = await _quizService.SearchQuizzesByNameAsync(quizName);
            return BaseResponse<List<QuizViewDto>>.OkResponse(quizzes);
        }

        // GET: api/quiz/paged
        [HttpGet("paged")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Retrieve quizzes with pagination.")]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<QuizViewDto>>>> GetQuizzesPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            BasePaginatedList<QuizViewDto> quizzes = await _quizService.GetQuizzesAsync(pageNumber, pageSize);
            return BaseResponse<BasePaginatedList<QuizViewDto>>.OkResponse(quizzes);
        }


    }
}
