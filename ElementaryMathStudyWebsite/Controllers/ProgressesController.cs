using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressesController : ControllerBase
    {
        private readonly IProgressService _progressService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        // Constructor
        public ProgressesController(IProgressService progressService, IUserService userService, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService)); ;
            _userService = userService ?? throw new ArgumentNullException(nameof(userService)); ;
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService)); ;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor)); ;
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
        public async Task<ActionResult<BasePaginatedList<Progress>>> GetStudentProgressByStudentId([Required] string studentId , int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                // Cast domain service to application service
                var progressAppService = _progressService as IAppProgressServices;
                var userAppService = _userService as IAppUserServices;

                // Get logged in User Id from authorization header 
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var currentUserId = _tokenService.GetUserIdFromTokenHeader(token).ToString().ToUpper();

                if (!await userAppService.IsCustomerChildren(currentUserId, studentId)) return BadRequest("They are not parent and child");

                BasePaginatedList<ProgressViewDto> subjectProgresses = await progressAppService.GetStudentProgressesDtoAsync(studentId, pageNumber, pageSize);
                return Ok(subjectProgresses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
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
        public async Task<ActionResult<BasePaginatedList<Progress>>> GetStudentProgress(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                // Cast domain service to application service
                var progressAppService = _progressService as IAppProgressServices;
                var userAppService = _userService as IAppUserServices;

                // Get logged in User Id from authorization header 
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var currentUserId = _tokenService.GetUserIdFromTokenHeader(token).ToString().ToUpper();

                BasePaginatedList<ProgressViewDto> subjectProgresses = await progressAppService.GetAllStudentProgressesDtoAsync(currentUserId, pageNumber, pageSize);
                return Ok(subjectProgresses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

    }
}
