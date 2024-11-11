using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Core.Base;
using Swashbuckle.AspNetCore.Annotations;
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

        [Authorize(Policy = "Admin-Content")]
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Gets all available questions.")]
        public async Task<ActionResult<BaseResponse<List<QuestionMainViewDto>>>> GetAllQuestions()
        {
            List<QuestionMainViewDto> questions = await _questionService.GetAllQuestionsMainViewDtoAsync();
            return BaseResponse<List<QuestionMainViewDto>>.OkResponse(questions);
        }

        //[Authorize(Policy = "Admin-Content")]
        //[HttpGet("search/question/{questionid}")]
        //[SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Gets a question by its id.")]
        //public async Task<ActionResult<BaseResponse<QuestionMainViewDto>>> GetQuestionById(string questionid)
        //{
        //    QuestionMainViewDto question = await _questionService.GetQuestionByIdAsync(questionid);
        //    return BaseResponse<QuestionMainViewDto>.OkResponse(question);
        //}

        [HttpGet("search/quiz/{quizId}")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Gets all questions related to a specific quiz.")]
        public async Task<ActionResult<BaseResponse<List<QuestionViewDto>>>> GetQuestionsByQuizId(string quizId)
        {
            List<QuestionViewDto> questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
            return BaseResponse<List<QuestionViewDto>>.OkResponse(questions);
        }

        [Authorize(Policy = "Admin-Content")]
        [HttpPost("add")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Adds a new question to the system.")]
        public async Task<ActionResult<BaseResponse<string>>> AddQuestionAsync(List<QuestionCreateDto> dtos)
        {
            BaseResponse<string> response = await _questionService.AddQuestionAsync(dtos);
            return Ok(response);
        }

        [Authorize(Policy = "Admin-Content")]
        [HttpPut("update/{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Updates a question by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<QuestionMainViewDto>>> UpdateQuestion(string id, QuestionUpdateDto dto)
        {
            QuestionMainViewDto updatedQuestion = await _questionService.UpdateQuestionAsync(id, dto);
            return BaseResponse<QuestionMainViewDto>.OkResponse(updatedQuestion, "Question udpated successfully");
        }

        [Authorize(Policy = "Admin-Content")]
        [HttpDelete("delete/{id}")]
        [SwaggerOperation(Summary = "Authorization: Admin & Content Manager", Description = "Deletes a question by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteQuestion(string id)
        {
            BaseResponse<string> response = await _questionService.DeleteQuestionAsync(id);
            return Ok(response);
        }

        [HttpGet("paginated")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Gets a paginated list of questions based on the provided page number and page size.")]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<QuestionViewDto>>>> GetQuestions(int pageNumber = 1, int pageSize = 10)
        {
            BasePaginatedList<QuestionViewDto> paginatedQuestions = await _questionService.GetQuestionsAsync(pageNumber, pageSize);
            return BaseResponse<BasePaginatedList<QuestionViewDto>>.OkResponse(paginatedQuestions);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Authorization: N/A", Description = "Searches for questions that contain the specified context.")]
        public async Task<ActionResult<BaseResponse<List<QuestionViewDto>>>> SearchQuestions(string context)
        {
            List<QuestionViewDto> questions = await _questionService.SearchQuestionsByContextAsync(context);
            return BaseResponse<List<QuestionViewDto>>.OkResponse(questions);
        }

    }
}
