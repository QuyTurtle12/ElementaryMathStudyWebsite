﻿using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Store;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressesController : ControllerBase
    {
        private readonly IAppProgressServices _progressService;
        private readonly IAppUserServices _userService;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor
        public ProgressesController(IAppProgressServices progressService, IAppUserServices userService, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _progressService = progressService;
            _userService = userService;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/progress/studentId
        // Get 1 child learning progress of specific parent
        [Authorize(Policy = "Parent")]
        [HttpGet]
        [Route("{studentId}")]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "View a child progress list. Insert -1 to get all items"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ProgressViewDto>>>> GetStudentProgressByStudentId([Required] string studentId , int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                // Get logged in User Id from authorization header 
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var currentUserId = _tokenService.GetUserIdFromTokenHeader(token).ToString().ToUpper();

                // Check if current logged user and the inputted student Id are parent-child relationship
                if (!await _userService.IsCustomerChildren(currentUserId, studentId))
                {
                    return BadRequest(new BaseResponse<BasePaginatedList<ProgressViewDto>>(
                    StatusCodeHelper.BadRequest,
                    StatusCodeHelper.BadRequest.Name(),
                    "They are not parent and child"
                    ));
                }

                BasePaginatedList<ProgressViewDto> subjectProgresses = await _progressService.GetStudentProgressesDtoAsync(studentId, pageNumber, pageSize);

                var response = BaseResponse<BasePaginatedList<ProgressViewDto>>.OkResponse(subjectProgresses);

                return response;
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }

        // GET: api/progress
        // Get 1 child learning progress of specific parent
        [Authorize(Policy = "Parent")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "View children progress list. Insert -1 to get all items"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ProgressViewDto>>>> GetStudentProgress(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                // Get logged in User Id from authorization header 
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var currentUserId = _tokenService.GetUserIdFromTokenHeader(token).ToString().ToUpper();

                BasePaginatedList<ProgressViewDto> subjectProgresses = await _progressService.GetAllStudentProgressesDtoAsync(currentUserId, pageNumber, pageSize);

                var response = BaseResponse<BasePaginatedList<ProgressViewDto>>.OkResponse(subjectProgresses);

                return response;
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }

        //// POST: api/progress
        //// Add student progress
        //[Authorize(Policy = "Student")]
        //[HttpPost]
        //[SwaggerOperation(
        //    Summary = "Authorization: Student",
        //    Description = "View children progress list. Insert -1 to get all items"
        //    )]
        //public async Task<ActionResult<BaseResponse<Progress>>> AddProgress(ProgressCreateDto progress)
        //{
        //    try
        //    {
        //        // Get logged in User Id from authorization header 
        //        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        //        var currentUserId = _tokenService.GetUserIdFromTokenHeader(token).ToString().ToUpper();

        //        // Validate before get to the main task
        //        string error = await _progressService.IsGenerallyValidatedAsync(progress.QuizId, currentUserId);
        //        if (!string.IsNullOrWhiteSpace(error))
        //        {
        //            throw new BaseException.BadRequestException("bad_request", error);
        //        }

        //        // Identify subject id using quiz id 
        //        string subjectId = await _progressService.GetSubjectIdFromQuizIdAsync(progress.QuizId); 

        //        Progress newStudentProgress = new Progress { StudentId = currentUserId, QuizId = progress.QuizId, SubjectId = subjectId };

        //        // Check 
        //        bool IsAddedProgress = await _progressService.AddSubjectProgressAsync(newStudentProgress);

        //        if (IsAddedProgress)
        //        {
        //            // Return only a success message
        //            var passedQuizResponse = BaseResponse<Progress>.OkResponse("Congratulations, you passed the quiz.Keep it up!");
        //            return passedQuizResponse;
        //        }

        //        // Return only a failure message
        //        var failedQuizResponse = BaseResponse<Progress>.OkResponse("You worked hard, but hard is not enough, try again next time");
        //        return failedQuizResponse;
        //    }
        //    catch (BaseException.CoreException coreEx)
        //    {
        //        // Handle specific CoreException
        //        return StatusCode(coreEx.StatusCode, new
        //        {
        //            code = coreEx.Code,
        //            message = coreEx.Message,
        //            additionalData = coreEx.AdditionalData
        //        });
        //    }
        //    catch (BaseException.BadRequestException badRequestEx)
        //    {
        //        // Handle specific BadRequestException
        //        return BadRequest(new
        //        {
        //            errorCode = badRequestEx.ErrorDetail.ErrorCode,
        //            errorMessage = badRequestEx.ErrorDetail.ErrorMessage
        //        });
        //    }
        //}

        // GET: api/progress/assigned-subject
        // Get list of assigned subject of specific student

        [Authorize(Policy = "Student")]
        [HttpGet]
        [Route("assigned-subject")]
        [SwaggerOperation(
            Summary = "Authorization: Student",
            Description = "View list of assigned subject. Insert -1 to get all items"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<AssignedSubjectDto>?>>> GetStudentAssignedSubjects(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                BasePaginatedList<AssignedSubjectDto>? assignedSubjectList = await _progressService.GetAssignedSubjectListAsync(pageNumber, pageSize);

                if (assignedSubjectList?.Items.Count == 0 || assignedSubjectList == null)
                {
                    throw new BaseException.BadRequestException("bad_request", "You don't have any assigned subject");
                }

                var haveAssignedSubjectResponse = BaseResponse<BasePaginatedList<AssignedSubjectDto>?>.OkResponse(assignedSubjectList);
                return haveAssignedSubjectResponse;
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
        }
    }
}
