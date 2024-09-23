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
        public async Task<ActionResult<List<QuestionViewDto>>> GetAllQuestions()
        {
            try
            {
                var questions = await _questionService.GetAllQuestionsAsync();
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/question/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get question by ID", Description = "Retrieve a specific question by its ID.")]
        public async Task<ActionResult<QuestionViewDto>> GetQuestionById(string id)
        {
            try
            {
                var question = await _questionService.GetQuestionByIdAsync(id);
                if (question == null)
                {
                    return NotFound(new { message = "Question not found." });
                }
                return Ok(question);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/question
        //[HttpPost]
        //[SwaggerOperation(Summary = "Add a new question", Description = "Create a new question.")]
        //public async Task<ActionResult<QuestionViewDto>> AddQuestion([FromBody] QuestionCreateDto dto)
        //{
        //    try
        //    {
        //        var question = await _questionService.AddQuestionAsync(dto);
        //        return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, question);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = ex.Message });
        //    }
        //}

        // GET: api/question/search
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search questions by context", Description = "Search for questions where the context contains the specified string.")]
        public async Task<ActionResult<List<QuestionViewDto>>> SearchQuestions([FromQuery] string context)
        {
            try
            {
                var questions = await _questionService.SearchQuestionsByContextAsync(context);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/question/quiz/{quizId}
        [HttpGet("quiz/{quizId}")]
        [SwaggerOperation(Summary = "Get questions by quiz ID", Description = "Retrieve questions that belong to a specific quiz.")]
        public async Task<ActionResult<List<QuestionViewDto>>> GetQuestionsByQuizId(string quizId)
        {
            try
            {
                var questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/question
        [HttpGet]
        [SwaggerOperation(Summary = "Get questions with pagination", Description = "Retrieve all questions with pagination.")]
        public async Task<ActionResult<BasePaginatedList<QuestionViewDto>>> GetQuestions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedQuestions = await _questionService.GetQuestionsAsync(pageNumber, pageSize);
                return Ok(paginatedQuestions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}