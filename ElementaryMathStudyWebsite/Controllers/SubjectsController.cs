using ElementaryMathStudyWebsite.Contract.Services.Interface;
using ElementaryMathStudyWebsite.Repositories.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();

            return Ok(subjects);
        }

        // GET: api/Subjects/{id}
        [HttpGet("{id}")]
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
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDTO subjectDTO)
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
            var subject = await _subjectService.CreateSubjectAsync(createdSubjectDTO);


            return CreatedAtAction(nameof(GetSubjectById), new { id = subject.Id }, createdSubjectDTO);
        }

        // PUT: api/Subjects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(string id, [FromBody] SubjectDTO subjectDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _subjectService.UpdateSubjectAsync(id, subjectDTO);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PUT: api/Subjects/ChangeStatus/{id}
        [HttpPut("/ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeSubjectStatus(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _subjectService.ChangeSubjectStatusAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
