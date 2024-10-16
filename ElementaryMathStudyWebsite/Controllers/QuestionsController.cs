﻿using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Core.Base;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpGet]
        [SwaggerOperation(Summary = "Retrieve all questions", Description = "Gets all available questions.")]
        public async Task<ActionResult<BaseResponse<List<QuestionMainViewDto>>>> GetAllQuestions()
        {
            List<QuestionMainViewDto> questions = await _questionService.GetAllQuestionsMainViewDtoAsync();
            return BaseResponse<List<QuestionMainViewDto>>.OkResponse(questions);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieve a question by its ID", Description = "Gets a question by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<QuestionMainViewDto>>> GetQuestionById(string id)
        {
            QuestionMainViewDto question = await _questionService.GetQuestionByIdAsync(id);
            return BaseResponse<QuestionMainViewDto>.OkResponse(question);
        }

        [HttpGet("quiz/{quizId}")]
        [SwaggerOperation(Summary = "Retrieve questions by Quiz ID", Description = "Gets all questions related to a specific quiz.")]
        public async Task<ActionResult<BaseResponse<List<QuestionViewDto>>>> GetQuestionsByQuizId(string quizId)
        {
            List<QuestionViewDto> questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
            return BaseResponse<List<QuestionViewDto>>.OkResponse(questions);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Add a new question", Description = "Adds a new question to the system.")]
        public async Task<ActionResult<BaseResponse<string>>> AddQuestionAsync(List<QuestionCreateDto> dtos)
        {
            BaseResponse<string> response = await _questionService.AddQuestionAsync(dtos);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing question", Description = "Updates a question by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<QuestionMainViewDto>>> UpdateQuestion(string id, QuestionUpdateDto dto)
        {
            QuestionMainViewDto updatedQuestion = await _questionService.UpdateQuestionAsync(id, dto);
            return BaseResponse<QuestionMainViewDto>.OkResponse(updatedQuestion);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a question", Description = "Deletes a question by its unique identifier.")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteQuestion(string id)
        {
            BaseResponse<string> response = await _questionService.DeleteQuestionAsync(id);
            return Ok(response);
        }

        [HttpGet("paginated")]
        [SwaggerOperation(Summary = "Retrieve paginated questions", Description = "Gets a paginated list of questions based on the provided page number and page size.")]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<QuestionViewDto>>>> GetQuestions(int pageNumber = 1, int pageSize = 10)
        {
            BasePaginatedList<QuestionViewDto> paginatedQuestions = await _questionService.GetQuestionsAsync(pageNumber, pageSize);
            return BaseResponse<BasePaginatedList<QuestionViewDto>>.OkResponse(paginatedQuestions);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search questions by context", Description = "Searches for questions that contain the specified context.")]
        public async Task<ActionResult<BaseResponse<List<QuestionViewDto>>>> SearchQuestions(string context)
        {
            List<QuestionViewDto> questions = await _questionService.SearchQuestionsByContextAsync(context);
            return BaseResponse<List<QuestionViewDto>>.OkResponse(questions);
        }

    }
}
