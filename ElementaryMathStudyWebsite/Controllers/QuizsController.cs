using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

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
        public async Task<ActionResult<List<Quiz>>> GetAllQuizzes()
        {
            try
            {
                var quizzes = await _quizService.GetAllQuizzesAsync();
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/quiz/{id}
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin & Manager", Description = "Retrieve a quiz by its unique identifier.")]
        public async Task<ActionResult<Quiz>> GetQuizById(string id)
        {
            try
            {
                var quiz = await _quizService.GetQuizByQuizIdAsync(id);
                if (quiz == null)
                {
                    return NotFound(new { message = "Quiz not found" });
                }
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/quiz/chapter/{chapterId}
        [HttpGet("chapter/{chapterId}")]
        [SwaggerOperation(Summary = "Authorization: Admin", Description = "Retrieve all quizzes belonging to a specific chapter.")]
        public async Task<ActionResult<List<QuizViewDto>>> GetQuizzesByChapterId(string chapterId)
        {
            try
            {
                var quizzes = await _quizService.GetQuizzesByChapterOrTopicIdAsync(chapterId, null);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/quiz/topic/{topicId}
        [HttpGet("topic/{topicId}")]
        [SwaggerOperation(Summary = "Authorization: Admin", Description = "Retrieve all quizzes belonging to a specific topic.")]
        public async Task<ActionResult<List<QuizViewDto>>> GetQuizzesByTopicId(string topicId)
        {
            try
            {
                var quizzes = await _quizService.GetQuizzesByChapterOrTopicIdAsync(null, topicId);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/quiz/search
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Authorization: Admin & Manager", Description = "Search for quizzes by name.")]
        public async Task<ActionResult<List<Quiz>>> SearchQuizzesByName([FromQuery, Required] string quizName)
        {
            try
            {
                var quizzes = await _quizService.SearchQuizzesByNameAsync(quizName);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/quiz/paged
        [HttpGet("paged")]
        [SwaggerOperation(Summary = "Authorization: Admin & Manager", Description = "Retrieve quizzes with pagination.")]
        public async Task<ActionResult<BasePaginatedList<QuizMainViewDto>>> GetQuizzesPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var quizzes = await _quizService.GetQuizzesAsync(pageNumber, pageSize);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/quiz
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new quiz.", Description = "Creates a new quiz and returns the created quiz.")]
        public async Task<ActionResult<QuizViewDto>> AddQuizAsync([FromBody] CreateQuizDto dto)
        {
            try
            {
                var createdQuiz = await _quizService.AddQuizAsync(dto);
                return CreatedAtAction(nameof(GetQuizById), new { id = createdQuiz.Id }, createdQuiz);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/quiz
        [HttpPut]
        [SwaggerOperation(Summary = "Update an existing quiz.", Description = "Updates an existing quiz based on the provided data.")]
        public async Task<ActionResult> UpdateQuizAsync([FromBody] UpdateQuizDto dto)
        {
            try
            {
                var success = await _quizService.UpdateQuizAsync(dto);
                if (!success)
                {
                    return NotFound(new { message = "Quiz not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/quiz/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a quiz.", Description = "Marks a quiz as deleted.")]
        public async Task<ActionResult> DeleteQuizAsync(string id)
        {
            try
            {
                var success = await _quizService.DeleteQuizAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Quiz not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}