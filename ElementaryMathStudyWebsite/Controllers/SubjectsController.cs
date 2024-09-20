using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController(IAppSubjectServices appSubjectServices) : ControllerBase
    {
        private readonly IAppSubjectServices _appSubjectServices = appSubjectServices;

        // GET: api/Subjects
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Get all active subjects with pagination; set pageSize to -1 to get all"
        )]
        public async Task<IActionResult> GetAllActiveSubjects(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var activeSubjects = await _appSubjectServices.GetAllSubjectsAsync(pageNumber, pageSize, false);
                if (activeSubjects?.Items.Count == 0 || activeSubjects == null)
                {
                    throw new BaseException.BadRequestException("no_subjects_found", "No active subjects found.");
                }

                return Ok(BaseResponse<BasePaginatedList<object>>.OkResponse(activeSubjects));
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // GET: api/Subjects/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Get subject by ID if it is active"
        )]
        public async Task<IActionResult> GetActiveSubjectById(string id)
        {
            try
            {
                var subject = await _appSubjectServices.GetSubjectByIDAsync(id, false);
                if (subject == null)
                {
                    throw new BaseException.BadRequestException("subject_not_found", "Subject not found.");
                }

                return Ok(subject);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // GET: api/Subjects/Admin
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("admin")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Get all subjects regardless of their status with pagination; set pageSize to -1 to get all"
        )]
        public async Task<IActionResult> GetAllSubjectsForAdmin(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var subjects = await _appSubjectServices.GetAllSubjectsAsync(pageNumber, pageSize, true); // true for admin access
                if (subjects?.Items.Count == 0 || subjects == null)
                {
                    throw new BaseException.BadRequestException("no_subjects_found", "No subjects found.");
                }

                var response = BaseResponse<BasePaginatedList<object>>.OkResponse(subjects);
                return Ok(response);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // GET: api/Subjects/Admin/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("admin/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Get subject by ID regardless of status"
        )]
        public async Task<IActionResult> GetSubjectByIdForAdmin(string id)
        {
            try
            {
                var subject = await _appSubjectServices.GetSubjectByIDAsync(id, true); // isAdmin = true
                if (subject == null)
                {
                    throw new BaseException.BadRequestException("subject_not_found", "The requested subject was not found.");
                }
                return Ok(subject);
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
        }


        // POST: api/Subjects
        [Authorize(Policy = "Admin-Content")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Create new subject"
        )]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectCreateDTO subjectDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSubject = await _appSubjectServices.CreateSubjectAsync(new SubjectDTO
                {
                    Id = "",
                    SubjectName = subjectDTO.SubjectName,
                    Price = subjectDTO.Price,
                    Status = true // Set status as active when created
                });

                return CreatedAtAction(nameof(GetActiveSubjectById), new { id = createdSubject.Id }, createdSubject);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // PUT: api/Subjects/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Update subject"
        )]
        public async Task<IActionResult> UpdateSubject(string id, [FromBody] SubjectDTO subjectDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedSubject = await _appSubjectServices.UpdateSubjectAsync(id, subjectDTO);
                return Ok(updatedSubject);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // PUT: api/Subjects/ChangeStatus/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpPut("/ChangeStatus/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Change subject status from true to false and otherwise."
        )]
        public async Task<IActionResult> ChangeSubjectStatus(string id)
        {
            try
            {
                var subject = await _appSubjectServices.ChangeSubjectStatusAsync(id);
                return Ok(subject);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // Search subjects by name, price
        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Search subject with filters and paginations, set price = -1 to not search for it"
        )]
        public async Task<IActionResult> SearchSubject([FromQuery] string searchTerm, double lowestPrice = -1, double highestPrice = -1, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var subjects = await _appSubjectServices.SearchSubjectAsync(searchTerm, lowestPrice, highestPrice, pageNumber, pageSize);
                return Ok(subjects);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }

        // Search subjects for admin
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("admin/search")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Search subject with filters and paginations, set price = -1, or leave status null to not search for it"
        )]
        public async Task<IActionResult> SearchSubjectAdmin([FromQuery] string searchTerm, double lowestPrice = -1, double highestPrice = -1, bool? status = null, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                return BadRequest("Search term must be at least 2 characters long.");
            }

            try
            {
                var subjects = await _appSubjectServices.SearchSubjectAdminAsync(searchTerm, lowestPrice, highestPrice, status, pageNumber, pageSize);
                if (subjects?.Items.Count == 0 || subjects == null)
                {
                    throw new BaseException.BadRequestException("no_subjects_found", "No subjects match the search criteria.");
                }

                var response = BaseResponse<BasePaginatedList<object>>.OkResponse(subjects);
                return Ok(response);
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
        }


        [HttpGet("check-subject-quiz")]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Check if user have complete the course or not"
        )]
        public async Task<IActionResult> CheckCompleteQuizExist(string subjectId, string quizId)
        {
            try
            {
                var exists = await _appSubjectServices.CheckCompleteQuizExistAsync(subjectId, quizId);
                if (!exists)
                {
                    throw new BaseException.BadRequestException("quiz_not_found", "No record found with the given SubjectId and QuizId for this user.");
                }
                return Ok(new { message = "Student has completed this course." });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
            }
            catch (BaseException.CoreException coreEx)
            {
                return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            }
        }
    }
}
