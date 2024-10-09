﻿using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IAppQuestionServices _questionService;
        // Constructor to inject IAppQuestionServices via dependency injection
        public QuestionController(IAppQuestionServices questionService)
        {
            _questionService = questionService;
        }

        // Get all questions
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<QuestionMainViewDto>>>> GetAllQuestions()
        {
            List<QuestionMainViewDto> questions = await _questionService.GetAllQuestionsMainViewDtoAsync();
            return BaseResponse<List<QuestionMainViewDto>>.OkResponse(questions);
        }

        // Get question by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<QuestionMainViewDto>>> GetQuestionById(string id)
        {
            QuestionMainViewDto question = await _questionService.GetQuestionByIdAsync(id);
            return BaseResponse<QuestionMainViewDto>.OkResponse(question);
        }

        // Search questions by context
        //[HttpGet("search")]
        //public async Task<ActionResult<BaseResponse<List<QuestionViewDto>>>> SearchQuestions(string context)
        //{
        //    List<QuestionViewDto> questions = await _questionService.SearchQuestionsByContextAsync(context);
        //    return BaseResponse<List<QuestionViewDto>>.OkResponse(questions);
        //}

        // Get questions by Quiz ID
        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<BaseResponse<List<QuestionViewDto>>>> GetQuestionsByQuizId(string quizId)
        {
            List<QuestionViewDto> questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
            return BaseResponse<List<QuestionViewDto>>.OkResponse(questions);
        }

        // Get questions with pagination 
        //[HttpGet("paginated")]
        //public async Task<ActionResult<BaseResponse<BasePaginatedList<QuestionViewDto>>>> GetQuestions(int pageNumber = 1, int pageSize = 10)
        //{
        //    BasePaginatedList<QuestionViewDto> paginatedQuestions = await _questionService.GetQuestionsAsync(pageNumber, pageSize);
        //    return BaseResponse<BasePaginatedList<QuestionViewDto>>.OkResponse(paginatedQuestions);
        //}

        // Add a new question using the data from QuestionCreateDto
        [HttpPost]
        public async Task<ActionResult<BaseResponse<string>>> AddQuestionAsync(QuestionCreateDto dto)
        {
            BaseResponse<string> response = await _questionService.AddQuestionAsync(dto);
            return Ok(response);
        }

        // Update an existing question by its ID using the data from QuestionUpdateDto
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<QuestionMainViewDto>>> UpdateQuestion(string id, QuestionUpdateDto dto)
        {
            QuestionMainViewDto updatedQuestion = await _questionService.UpdateQuestionAsync(id, dto);
            return BaseResponse<QuestionMainViewDto>.OkResponse(updatedQuestion);
        }

        //Delete a question by its ID
       [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteQuestion(string id)
        {
            BaseResponse<string> response = await _questionService.DeleteQuestionAsync(id);
            return Ok(response);
        }
    }
}