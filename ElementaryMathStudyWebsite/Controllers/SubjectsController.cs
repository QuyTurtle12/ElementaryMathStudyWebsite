using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
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

                var response = BaseResponse<object>.OkResponse(subject);
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

                var response = BaseResponse<object>.OkResponse(subject);
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

                var response = BaseResponse<SubjectAdminViewDTO>.OkResponse(createdSubject);
                return CreatedAtAction(nameof(GetActiveSubjectById), new { id = createdSubject.Id }, response);
            }
            catch (ArgumentException argEx)
            {
                // Handle argument exceptions such as validation errors
                return StatusCode(400, new
                {
                    errorMessage = "An unexpected error occurred.",
                    details = argEx.Message
                });
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
            catch (Exception ex)
            {
                // Catch all other exceptions and return a generic server error
                return StatusCode(500, new
                {
                    errorMessage = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        // PUT: api/Subjects/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Update subject"
        )]
        public async Task<IActionResult> UpdateSubject(string id, [FromBody] SubjectUpdateDTO subjectDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedSubject = await _appSubjectServices.UpdateSubjectAsync(id, subjectDTO);
                var response = BaseResponse<SubjectAdminViewDTO>.OkResponse(updatedSubject);
                return Ok(response);
            }
            catch (ArgumentException argEx)
            {
                // Handle argument exceptions such as validation errors
                return StatusCode(400, new
                {
                    errorMessage = "An unexpected error occurred.",
                    details = argEx.Message
                });
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
            catch (Exception ex)
            {
                // Catch all other exceptions and return a generic server error
                return StatusCode(500, new
                {
                    errorMessage = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        // PUT: api/Subjects/ChangeStatus/{id}
        //[Authorize(Policy = "Admin-Content")]
        //[HttpPut("/ChangeStatus/{id}")]
        //[SwaggerOperation(
        //    Summary = "Authorization: Admin, Content Manager",
        //    Description = "Change subject status from true to false and otherwise."
        //)]
        //public async Task<IActionResult> ChangeSubjectStatus(string id)
        //{
        //    try
        //    {
        //        var subject = await _appSubjectServices.ChangeSubjectStatusAsync(id);
        //        if (subject == null)
        //        {
        //            throw new BaseException.BadRequestException("subject_not_found", "The requested subject was not found.");
        //        }
        //        var response = BaseResponse<SubjectAdminViewDTO>.OkResponse(subject);
        //        return Ok(response);
        //    }
        //    catch (BaseException.CoreException coreEx)
        //    {
        //        return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
        //    }
        //    catch (BaseException.BadRequestException badRequestEx)
        //    {
        //        return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
        //    }
        //}

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
                throw new BaseException.BadRequestException("search_term_error", "Search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term must be at least 2 characters long.");
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

        // DELETE: api/Subjects/SoftDelete/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpDelete("SoftDelete/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Soft delete a subject by setting DeletedBy and DeletedTime."
        )]
        public async Task<IActionResult> SoftDeleteSubject(string id)
        {
            try
            {
                await _appSubjectServices.SoftDeleteSubjectAsync(id);
                return Ok(BaseResponse<object>.OkResponse(new { message = "Subject has been successfully soft deleted." }));
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

        // PUT: api/Subjects/RestoreSoftDelete/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpPut("RestoreSoftDelete/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Resote soft delete a subject by setting DeletedBy and DeletedTime."
        )]
        public async Task<IActionResult> RestoreSoftDeleteSubject(string id)
        {
            try
            {
                await _appSubjectServices.RestoreSubjectAsync(id);
                return Ok(BaseResponse<object>.OkResponse(new { message = "Subject has been successfully restored." }));
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
    }
}
