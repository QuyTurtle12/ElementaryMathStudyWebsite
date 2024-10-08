﻿using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAnswersController(IAppUserAnswerServices userAnswerService) : ControllerBase
    {
        private readonly IAppUserAnswerServices _userAnswerService = userAnswerService ?? throw new ArgumentNullException(nameof(userAnswerService));

        // GET: api/UserAnswers
        //[Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Get all the user answers by paging, pageSize = -1 to get all"
        )]
        [HttpGet]
        public async Task<IActionResult> GetAllUserAnswers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userAnswerService.GetAllUserAnswersAsync(pageNumber, pageSize);
            var response = BaseResponse<BasePaginatedList<UserAnswerWithDetailsDTO>>.OkResponse(result);
            return Ok(response);
        }

        [Authorize(Policy = "Student")]
        [SwaggerOperation(
            Summary = "Authorization: Student",
            Description = "When user answers create an instance of it"
        )]
        [HttpPost]
        [Route("create")]
        [HttpPost]
        public async Task<ActionResult<BaseResponse<UserAnswer>>> CreateUserAnswers([FromBody] UserAnswerCreateDTO userAnswerCreateDTO)
        {
            if (userAnswerCreateDTO == null || !userAnswerCreateDTO.UserAnswerList.Any())
            {
                throw new BaseException.BadRequestException("input_error", "Invalid input. The user answer list is empty or null.");
            }

            var createdUserAnswers = await _userAnswerService.CreateUserAnswersAsync(userAnswerCreateDTO);

            if (!createdUserAnswers.IsAddedResult)
            {
                throw new BaseException.CoreException("error", "Failed to add student result");
            }

            if (createdUserAnswers.IsPassedTheQuiz)
            {
                // Return only a success message
                var passedQuizResponse = BaseResponse<UserAnswer>.OkResponse("Congratulations, you passed the quiz.Keep it up!");
                return passedQuizResponse;
            }

            // Return only a failure message
            var failedQuizResponse = BaseResponse<UserAnswer>.OkResponse("You worked hard, but hard is not enough, try again next time");
            return failedQuizResponse;
        }

        // GET: api/QuizAnswers/quiz/{quizId}
        [Authorize(Policy = "Student")]
        [SwaggerOperation(
            Summary = "Authorization: Student",
            Description = "Get all answers from the current student for the given quiz"
        )]
        [HttpGet("quiz/{quizId}")]
        public async Task<IActionResult> GetUserAnswersByQuizId(string quizId)
        {
            var userAnswers = await _userAnswerService.GetUserAnswersByQuizIdAsync(quizId);
            var response = BaseResponse<BasePaginatedList<UserAnswerWithDetailsDTO>>.OkResponse(userAnswers);
            return Ok(response);
        }
    }
}
