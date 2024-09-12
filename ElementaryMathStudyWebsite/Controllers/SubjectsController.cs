using ElementaryMathStudyWebsite.Contract.Services.Interface;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Drawing.Printing;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly IAppSubjectServices _appSubjectServices;

        public SubjectsController(ISubjectService subjectService, IAppSubjectServices appSubjectServices)
        {
            _subjectService = subjectService;
            _appSubjectServices = appSubjectServices;
        }

        // GET: api/Subjects
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Get all subjects with pagination; set pageSize to -1 to get all"
        )]
        public async Task<IActionResult> GetAllSubjects(int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize != -1 && (pageNumber < 1 || pageNumber > pageSize))
            {
                return BadRequest("pageNumber must be between 1 and " + pageSize);
            }

            var subjects = pageSize > 0 ? (await _subjectService.GetAllSubjectsAsync())
                                  .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize) : await _subjectService.GetAllSubjectsAsync();

            return Ok(subjects);
        }

        // GET: api/Subjects/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Get subject by ID"
        )]
        public async Task<IActionResult> GetSubjectById(string id)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIDAsync(id);

                var subjectDTO = new SubjectDTO
                {
                    SubjectName = subject.SubjectName,
                    Price = subject.Price,
                    Status = subject.Status
                };

                return Ok(subjectDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/Subjects
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

            var createdSubjectDTO = new SubjectDTO
            {
                SubjectName = subjectDTO.SubjectName,
                Price = subjectDTO.Price,
                Status = true
            };
            var subject = await _appSubjectServices.CreateSubjectAsync(createdSubjectDTO);


            return CreatedAtAction(nameof(GetSubjectById), new { id = subject.Id }, createdSubjectDTO);
        }

        // PUT: api/Subjects/{id}
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
                var subject = await _appSubjectServices.UpdateSubjectAsync(id, subjectDTO);
                return Ok(new {subject.Id, subject.SubjectName, subject.Price, subject.Status});
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PUT: api/Subjects/ChangeStatus/{id}
        [HttpPut("/ChangeStatus/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Change subject status from true to false and otherwise."
        )]
        public async Task<IActionResult> ChangeSubjectStatus(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var subject = await _appSubjectServices.ChangeSubjectStatusAsync(id);
                return Ok(new {subject.Id, subject.SubjectName, subject.Price, subject.Status});
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Search subjects by name
        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Search subject by name, pageSize = -1 to have it show all."
        )]
        public async Task<IActionResult> SearchSubject([FromQuery] string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            // Validate the search term
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                return BadRequest("Search term must be at least 2 characters long.");
            }

            if (pageSize != -1 && (pageNumber < 1 || pageNumber > pageSize))
            {
                return BadRequest("pageNumber must be between 1 and " + pageSize);
            }

            try
            {
                var subjects = (await _appSubjectServices.SearchSubjectAsync(searchTerm))
                                                    .Skip((pageNumber - 1) * pageSize)
                                                    .Take(pageSize);
                return Ok(subjects);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle case when no subjects are found
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
