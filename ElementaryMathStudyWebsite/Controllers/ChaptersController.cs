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
    public class ChaptersController(IAppChapterServices appChapterServices) : ControllerBase
    {
        private readonly IAppChapterServices _chapterService = appChapterServices;

        // GET: api/chapters/manager
        // Get chapters for Manager & Admin
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("manager")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "View chapter list for Manager and Admin Role. Insert -1 to get all items"
        )]
        public async Task<ActionResult<BasePaginatedList<Chapter?>>> GetChapters(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                var chapterAppService = _chapterService as IAppChapterServices;
                return Ok(await chapterAppService.GetChaptersAsync(pageNumber, pageSize));
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
                // Handle any other exceptions
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        // GET: api/chapters/manager/{id}
        // Get chapters for Manager & Admin
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("manager/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "View chapter for Manager and Admin Role."
            )]
        public async Task<IActionResult> GetChapter(string id)
        {
            try
            {
                var chapter = await _chapterService.GetChapterByChapterIdAsync(id);
                if (chapter == null)
                {
                    return NotFound(new
                    {
                        error = "Invalid Chapter Id",
                        message = "The chapter with the specified ID was not found."
                    });
                }
                return Ok(chapter);
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
                // Handle any other exceptions
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }


        // GET: api/chapters/{id}
        // Get chapters for general user
        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "View chapter for General User"
            )]
        public async Task<ActionResult<ChapterViewDto?>> GetChapterForGeneralUser([Required] string id)
        {
            try
            {
                ChapterViewDto? chapter = await _chapterService.GetChapterDtoByChapterIdAsync(id);
                if (chapter == null)
                {
                    return NotFound(new
                    {
                        error = "Invalid Chapter Id",
                        message = "The chapter with the specified ID was not found."
                    });
                }
                return Ok(chapter);
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
                // Handle any other exceptions
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        // GET: api/chapter
        // Get chapters for general user
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "View chapter list for General User. Insert -1 to get all items"
            )]
        public async Task<ActionResult<BasePaginatedList<ChapterViewDto>>> GetChaptersForGeneralUser(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                BasePaginatedList<ChapterViewDto?> chapters = await _chapterService.GetChapterDtosAsync(pageNumber, pageSize);
                return Ok(chapters);
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
                // Handle any other exceptions
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("subject")]
        [SwaggerOperation(
           Summary = "Authorization: N/A",
           Description = "View all chapters of 1 subject"
           )]
        public async Task<IActionResult> GetChapterBySubjectId([Required] string subjectId, int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                var chapterAppService = _chapterService as IAppChapterServices;
                var chapters = await chapterAppService.GetChaptersBySubjectIdAsync(pageNumber, pageSize, subjectId);
                return Ok(chapters);
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
                // Handle any other exceptions
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }


        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Search chapter by name, pageSize = -1 to have it show all."
        )]
        public async Task<IActionResult> SearchChapter([FromQuery] string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            // Validate the search term
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new
                {
                    error = "Invalid Search Term",
                    message = "Search term cannot be empty."
                });
            }

            if (searchTerm.Length < 2)
            {
                return BadRequest(new
                {
                    error = "Invalid Search Term",
                    message = "Search term must be at least 2 characters long."
                });
            }

            try
            {
                var chapters = await _chapterService.SearchChapterAsync(searchTerm, pageNumber, pageSize);
                return Ok(chapters);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle case when no chapters are found
                return NotFound(new
                {
                    error = "Not Found",
                    message = ex.Message
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
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    error = "Internal Server Error",
                    message = ex.Message
                });
            }
        }

        [Authorize(Policy = "Admin-Manager")]
        [HttpGet("search/admin")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Search chapter by name for admin, pageSize = -1 to have it show all."
        )]
        public async Task<IActionResult> SearchChapterForAdmin([FromQuery] string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            // Validate the search term
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new
                {
                    error = "Invalid Search Term",
                    message = "Search term cannot be empty."
                });
            }

            if (searchTerm.Length < 2)
            {
                return BadRequest(new
                {
                    error = "Invalid Search Term",
                    message = "Search term must be at least 2 characters long."
                });
            }

            try
            {
                var chapters = await _chapterService.SearchChapterForAdminAsync(searchTerm, pageNumber, pageSize);
                return Ok(chapters);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle case when no chapters are found
                return NotFound(new
                {
                    error = "Not Found",
                    message = ex.Message
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
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    error = "Internal Server Error",
                    message = ex.Message
                });
            }
        }

        // POST: api/chapters/
        // Add chapters
        [Authorize(Policy = "Admin-Manager")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Create new chapter"
        )]
        public async Task<IActionResult> CreateChapter([FromBody] ChapterDto chapterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    error = "Invalid Model State",
                    details = ModelState
                });
            }

            try
            {
                var createdChapter = await _chapterService.CreateChapterAsync(new ChapterDto
                {
                    ChapterName = chapterDTO.ChapterName,
                    //Status = true, // Set status as active when created
                    SubjectId = chapterDTO.SubjectId,
                    QuizId = chapterDTO.QuizId,
                });

                return CreatedAtAction(nameof(GetChapterForGeneralUser), new { id = createdChapter.Id }, createdChapter);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    error = "Invalid Operation",
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = "Argument Error",
                    message = ex.Message
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
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    error = "Internal Server Error",
                    message = ex.Message
                });
            }
        }

        [Authorize(Policy = "Admin-Manager")]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Update chapter"
        )]
        public async Task<IActionResult> UpdateChapter(string id, [FromBody] ChapterDto chapterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    error = "Invalid Model State",
                    details = ModelState
                });
            }

            try
            {
                var chapter = await _chapterService.UpdateChapterAsync(id, chapterDTO);
                return Ok(chapter);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    error = "Invalid Operation",
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = "Argument Error",
                    message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    error = "Not Found",
                    message = ex.Message
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
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    error = "Internal Server Error",
                    message = ex.Message
                });
            }
        }


        [Authorize(Policy = "Admin-Content")]
        [HttpPut("/StatusChange/{id}")]
        [SwaggerOperation(
    Summary = "Authorization: Admin, Content Manager",
    Description = "Change chapter status from true to false and otherwise."
)]
        public async Task<IActionResult> ChangeChapterStatus(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    error = "Invalid Model State",
                    details = ModelState
                });
            }

            try
            {
                var chapter = await _chapterService.ChangeChapterStatusAsync(id);
                return Ok(chapter);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    error = "Not Found",
                    message = ex.Message
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
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    error = "Internal Server Error",
                    message = ex.Message
                });
            }
        }


        [Authorize(Policy = "Admin-Manager")]
        [HttpPut("/rollbakChapter/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Rollback chapter was deleted"
        )]
        public async Task<IActionResult> rollbackChapterDeleted([Required] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    error = "Invalid Model State",
                    details = ModelState
                });
            }

            try
            {
                var chapter = await _chapterService.rollbackChapterDeletedAsync(id);
                return Ok(chapter);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    error = "Not Found",
                    message = ex.Message
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
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    error = "Internal Server Error",
                    message = ex.Message
                });
            }
        }


        [Authorize(Policy = "Admin-Manager")]
        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Delete a chapter"
        )]
        public async Task<IActionResult> DeleteChapter([Required] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    error = "Invalid Model State",
                    details = ModelState
                });
            }

            try
            {
                var chapter = await _chapterService.DeleteChapterAsync(id);
                return Ok(chapter);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    error = "Not Found",
                    message = ex.Message
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
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    error = "Internal Server Error",
                    message = ex.Message
                });
            }
        }


        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("manager/deleted")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "View list chapter was deleted for Manager and Admin Role. Insert -1 to get all items"
        )]
        public async Task<ActionResult<BasePaginatedList<Chapter?>>> GetChaptersDeleted(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                var chapterAppService = _chapterService as IAppChapterServices;
                var chapters = await chapterAppService.GetChaptersDeletedAsync(pageNumber, pageSize);
                return Ok(chapters);
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
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    error = "Internal Server Error",
                    message = ex.Message
                });
            }
        }


    }
}