using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
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
            var activeSubjects = await _appSubjectServices.GetAllSubjectsAsync(pageNumber, pageSize, false);

            return Ok(activeSubjects);
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
                var subject = await _appSubjectServices.GetSubjectByIDAsync(id, false); //not Admin
                return Ok(subject);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
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
            var subjects = await _appSubjectServices.GetAllSubjectsAsync(pageNumber, pageSize, true); //true mean it was admin

            return Ok(subjects);
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
                var subject = await _appSubjectServices.GetSubjectByIDAsync(id, true); //is Admin
                return Ok(subject);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
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
                    SubjectName = subjectDTO.SubjectName,
                    Price = subjectDTO.Price,
                    Status = true // Set status as active when created
                });

                return CreatedAtAction(nameof(GetSubjectByIdForAdmin), new { id = createdSubject.Id }, createdSubject);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Return the error message if a duplicate name is found
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return the error message if a validation error is found
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
                var subject = await _appSubjectServices.UpdateSubjectAsync(id, subjectDTO);
                return Ok(subject);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Return the error message if a duplicate name is found
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return the error message if a validation error is found
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // Return 404 if the subject is not found
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var subject = await _appSubjectServices.ChangeSubjectStatusAsync(id);
                return Ok(subject);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Search subjects by name, price
        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Search subject with filters and paginations, set price = -1 to not search for it"
        )]
        public async Task<IActionResult> SearchSubject([FromQuery] string searchTerm, double lowestPrice = -1,
                double highestPrice = -1, int pageNumber = 1, int pageSize = 10)
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

            try
            {
                var subjects = await _appSubjectServices.SearchSubjectAsync(searchTerm, lowestPrice, highestPrice, pageNumber, pageSize);
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

        // Search subjects for admin
        [Authorize(Policy = "Admin-Content")]
        [HttpGet("admin/search")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Search subject with filters and paginations, set price = -1, or leave status null to not search for it"
        )]
        public async Task<IActionResult> SearchSubjectAdmin([FromQuery] string searchTerm, double lowestPrice = -1,
                double highestPrice = -1, bool? status = null, int pageNumber = 1, int pageSize = 10)
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

            try
            {
                var subjects = await _appSubjectServices.SearchSubjectAdminAsync(searchTerm, lowestPrice, highestPrice, status, pageNumber, pageSize);
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


        [HttpGet("check-subject-quiz")]
        [SwaggerOperation(
            Summary = "Authorization: Anyone",
            Description = "Check if user have complete the course or not"
        )]
        public async Task<IActionResult> CheckCompleteQuizExist(string subjectId, string quizId)
        {
            var exists = await _appSubjectServices.CheckCompleteQuizExistAsync(subjectId, quizId);

            if (exists)
            {
                return Ok(new { message = "Student have complete this course." });
            }

            return NotFound(new { message = "No record found with the given SubjectId and QuizId of this user." });
        }
    }
}
