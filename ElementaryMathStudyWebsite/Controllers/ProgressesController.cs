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
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressesController : ControllerBase
    {
        private readonly IAppProgressServices _progressService;
        private readonly IAppUserServices _userService;

        // Constructor
        public ProgressesController(IAppProgressServices progressService, IAppUserServices userService)
        {
            _progressService = progressService;
            _userService = userService;
        }

        // GET: api/progress//parent/view/{studentId}
        // Get 1 child learning progress of specific parent
        [Authorize(Policy = "Parent")]
        [HttpGet]
        [Route("/parent/view/{studentId}")]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "View a child progress list for Parent role. Insert -1 to get all items"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ProgressViewDto>>>> GetStudentProgressByStudentId([Required] string studentId , int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                // Get logged in User info
                User currentUser = await _userService.GetCurrentUserAsync();

                // Check if current logged user and the inputted student Id are parent-child relationship
                if (!await _userService.IsCustomerChildren(currentUser.Id, studentId))
                {
                    throw new BaseException.BadRequestException("invalid_argument", "They are not parent and child");
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

        // GET: api/progress/student/view
        // Get 1 child learning progress of specific parent
        [Authorize(Policy = "Student")]
        [HttpGet]
        [Route("/student/view")]
        [SwaggerOperation(
            Summary = "Authorization: Student",
            Description = "View a child progress list for Student role. Insert -1 to get all items"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ProgressViewDto>>>> GetStudentProgressForStudent(int pageNumber = -1, int pageSize = -1)
        {
            try
            {

                BasePaginatedList<ProgressViewDto> subjectProgresses = await _progressService.GetStudentProgressesDtoForStudentAsync(pageNumber, pageSize);

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

        // GET: api/progress/parent/view
        // Get 1 child learning progress of specific parent
        [Authorize(Policy = "Parent")]
        [HttpGet]
        [Route("/parent/view")]
        [SwaggerOperation(
            Summary = "Authorization: Parent",
            Description = "View children progress list. Insert -1 to get all items"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ProgressViewDto>>>> GetStudentProgress(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                // Get logged in User info
                User currentUser = await _userService.GetCurrentUserAsync();

                BasePaginatedList<ProgressViewDto> subjectProgresses = await _progressService.GetAllStudentProgressesDtoAsync(currentUser.Id, pageNumber, pageSize);

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

        // GET: api/progress/student/view/assigned-subject
        // Get list of assigned subject of specific student

        [Authorize(Policy = "Student")]
        [HttpGet]
        [Route("/student/view/assigned-subject")]
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
