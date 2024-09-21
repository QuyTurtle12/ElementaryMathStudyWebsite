using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IAppQuestionServices _questionService;

        public QuestionsController(IAppQuestionServices questionService)
        {
            _questionService = questionService;
        }

        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization:  Admin-Manager",
            Description = "View question list"
            )]
        public async Task<ActionResult<IList<QuestionDto>>> GetAllQuestions()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        // GET api/question/5
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "View question"
            )]
        public async Task<ActionResult<QuestionDto>> GetQuestionById(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionByIdAsync(id);
                return Ok(question);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST api/question
        [Authorize(Policy = "Admin-Manager")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Create Question"
            )]
        public async Task<ActionResult<QuestionDto>> CreateQuestion(CreateQuestionDto dto)
        {
            var question = await _questionService.CreateQuestionAsync(dto);
            var id = question.Id;
            return CreatedAtAction(nameof(GetQuestionById), new { id = id }, question);
        }

        // PUT api/question/5
        [Authorize(Policy = "Admin-Manager")]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Update Question"
            )]
        public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto dto)
        {
            try
            {
                var success = await _questionService.UpdateQuestionAsync(id, dto);
                if (success)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // DELETE api/question/5
        [Authorize(Policy = "Admin-Manager")]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Delete Question"
            )]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                var success = await _questionService.DeleteQuestionAsync(id);
                if (success)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
