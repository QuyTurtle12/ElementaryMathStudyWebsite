using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizsController : ControllerBase
    {
        private readonly IAppQuizServices _quizService;

        public QuizsController(IAppQuizServices QuizService)
        {
            _quizService = QuizService ?? throw new ArgumentNullException(nameof(QuizService));
        }


        [HttpGet("quizzes")]
        public async Task<ActionResult<IList<QuizViewDto?>>> GetQuizzes()
        {
            try
            {
                IList<QuizViewDto> quizzes = await _quizService.GetQuizzesAsync();
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("quiz/{id}")]
        public async Task<ActionResult<QuizDetailsDto>> GetQuiz([Required] string id)
        {
            try
            {
                QuizDetailsDto? quizDetails = await _quizService.GetQuizByQuizIdAsync(id);
                if (quizDetails == null)
                {
                    return BadRequest("Invalid Quiz Id");
                }
                return Ok(quizDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("quiz/search")]
        public async Task<ActionResult<IList<QuizViewDto>>> SearchQuizzes([FromQuery] string? quizName = null, [FromQuery] double? criteria = null)
        {
            try
            {
                var quizzes = await _quizService.SearchQuizzesAsync(quizName, criteria);

                if (quizzes == null)
                {
                    return NotFound("No quizzes found matching the search criteria.");
                }

                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("quiz")]
        public async Task<ActionResult<QuizViewDto>> AddQuiz([FromBody] QuizCreateDto quizCreateDto)
        {
            try
            {
                QuizViewDto quizViewDto = await _quizService.AddQuizAsync(quizCreateDto);
                return Ok(quizViewDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpDelete]
        [Route("quiz/{id}")]
        public async Task<IActionResult> DeleteQuiz([Required] string id)
        {
            try
            {
                bool result = await _quizService.DeleteQuizAsync(id);
                if (result)
                {
                    return NoContent();
                }
                return NotFound("Quiz does not exist.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

    }
}
