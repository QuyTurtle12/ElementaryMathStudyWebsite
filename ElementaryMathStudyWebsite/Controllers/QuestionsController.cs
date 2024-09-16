using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        // GET api/question
        [HttpGet]
        public async Task<ActionResult<IList<QuestionDto>>> GetAllQuestions()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        // GET api/question/5
        [HttpGet("{id}")]
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
        [HttpPost]
        public async Task<ActionResult<QuestionDto>> CreateQuestion(CreateQuestionDto dto)
        {
            var question = await _questionService.CreateQuestionAsync(dto);
            var id = question.Id;
            return CreatedAtAction(nameof(GetQuestionById), new { id = id }, question);
        }

        // PUT api/question/5
        [HttpPut("{id}")]
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

        //// DELETE api/question/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteQuestion(int id)
        //{
        //    try
        //    {
        //        var success = await _questionService.DeleteQuestionAsync(id);
        //        if (success)
        //        {
        //            return NoContent();
        //        }
        //        return NotFound();
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}
    }
}
