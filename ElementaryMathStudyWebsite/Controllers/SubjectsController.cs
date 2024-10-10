using AutoMapper;
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
    public class SubjectsController(IAppSubjectServices appSubjectServices, IMapper mapper) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly IAppSubjectServices _appSubjectServices = appSubjectServices;

        // GET: api/Subjects
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Get all active subjects with pagination; set pageSize to -1 to get all"
        )]
        public async Task<IActionResult> GetAllActiveSubjects(int pageNumber = 1, int pageSize = 10)
        {
            var activeSubjects = await _appSubjectServices.GetAllSubjectsAsync(pageNumber, pageSize, false);
            return Ok(BaseResponse<BasePaginatedList<object>>.OkResponse(activeSubjects));
        }

        // GET: api/Subjects/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Get subject by ID if it is active"
        )]
        public async Task<IActionResult> GetActiveSubjectById(string id)
        {
            var subject = await _appSubjectServices.GetSubjectByIDAsync(id, false);
            var response = BaseResponse<object>.OkResponse(subject);
            return Ok(subject);
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
            var subjects = await _appSubjectServices.GetAllSubjectsAsync(pageNumber, pageSize, true); // true for admin access
            var response = BaseResponse<BasePaginatedList<object>>.OkResponse(subjects);
            return Ok(response);
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
            var subject = await _appSubjectServices.GetSubjectByIDAsync(id, true); // isAdmin = true

            var response = BaseResponse<object>.OkResponse(subject);
            return Ok(response);
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

            var newSubject = _mapper.Map<SubjectDTO>(subjectDTO);
            //var createdSubject = await _appSubjectServices.CreateSubjectAsync(newSubject);
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

            var updatedSubject = await _appSubjectServices.UpdateSubjectAsync(id, subjectDTO);
            var response = BaseResponse<SubjectAdminViewDTO>.OkResponse(updatedSubject);
            return Ok(response);
        }

        // Search subjects by name, price
        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Search subject with filters and paginations, set price = -1 to not search for it"
        )]
        public async Task<IActionResult> SearchSubject([FromQuery] string searchTerm, double lowestPrice = -1, double highestPrice = -1, int pageNumber = 1, int pageSize = 10)
        {
            var subjects = await _appSubjectServices.SearchSubjectAsync(searchTerm, lowestPrice, highestPrice, pageNumber, pageSize);
            var response = BaseResponse<BasePaginatedList<object>>.OkResponse(subjects);
            return Ok(response);
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
            var subjects = await _appSubjectServices.SearchSubjectAdminAsync(searchTerm, lowestPrice, highestPrice, status, pageNumber, pageSize);
            var response = BaseResponse<BasePaginatedList<object>>.OkResponse(subjects);
            return Ok(response);
        }

        //// DELETE: api/Subjects/SoftDelete/{id}
        //[Authorize(Policy = "Admin-Content")]
        //[HttpDelete("SoftDelete/{id}")]
        //[SwaggerOperation(
        //    Summary = "Authorization: Admin, Content Manager",
        //    Description = "Soft delete a subject by setting DeletedBy and DeletedTime."
        //)]
        //public async Task<IActionResult> SoftDeleteSubject(string id)
        //{
        //    await _appSubjectServices.SoftDeleteSubjectAsync(id);
        //    return Ok(BaseResponse<object>.OkResponse(new { message = "Subject has been successfully soft deleted." }));
        //}

        // PUT: api/Subjects/RestoreSoftDelete/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpPut("RestoreSoftDelete/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Resote soft delete a subject by setting DeletedBy and DeletedTime."
        )]
        public async Task<IActionResult> RestoreSoftDeleteSubject(string id)
        {
            await _appSubjectServices.RestoreSubjectAsync(id);
            return Ok(BaseResponse<object>.OkResponse(new { message = "Subject has been successfully restored." }));
        }
    }
}
