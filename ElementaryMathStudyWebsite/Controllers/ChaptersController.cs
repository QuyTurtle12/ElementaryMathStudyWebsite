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
    public class ChaptersController : ControllerBase
    {
        private readonly IAppChapterServices _chapterService;

        public ChaptersController(IAppChapterServices chapterService)
        {
            _chapterService = chapterService ?? throw new ArgumentNullException(nameof(chapterService));
        }

        // GET: api/chapters/manager
        // Get chapters for Manager & Admin
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("manager")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "View chapter list for Manager and Admin Role. Insert -1 to get all items"
        )]
        public async Task<ActionResult<BasePaginatedList<Chapter>>> GetChapters(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                BasePaginatedList<Chapter> chapters = await _chapterService.GetChaptersAsync(pageNumber, pageSize);
                return Ok(chapters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Invalid input: " + ex.Message);
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
        public async Task<ActionResult<Chapter>> GetChapter([Required] string id)
        {
            try
            {
                Chapter chapter = await _chapterService.GetChapterByChapterIdAsync(id);
                if (chapter == null)
                {
                    return BadRequest("Invalid Chapter Id");
                }
                return Ok(chapter);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
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
        public async Task<ActionResult<ChapterViewDto>> GetChapterForGeneralUser([Required] string id)
        {
            try
            {

                ChapterViewDto chapter = await _chapterService.GetChapterDtoByChapterIdAsync(id);
                if (chapter == null)
                {
                    return BadRequest("Invalid Chapter Id");
                }
                return Ok(chapter);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        // GET: api/chapter
        // Get chapters for general user
        [HttpGet]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "View chapter list for General User. Insert -1 to get all items"
            )]
        public async Task<ActionResult<BasePaginatedList<OrderViewDto>>> GetChaptersForGeneralUser(int pageNumber = -1, int pageSize = -1)
        {
            try
            {

                BasePaginatedList<ChapterViewDto> chapters = await _chapterService.GetChapterDtosAsync(pageNumber, pageSize);
                return Ok(chapters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
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
        public async Task<IActionResult> CreateChapter([FromBody] ChapterAddDto chapterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                var createdChapter = await _chapterService.CreateChapterAsync(new ChapterDto
                {
                    Number = chapterDTO.Number,
                    ChapterName = chapterDTO.ChapterName,
                    Status = true, // Set status as active when created
                    SubjectId = chapterDTO.SubjectId,
                    QuizId = chapterDTO.QuizId,
                });

                return CreatedAtAction(nameof(GetChapter), new { id = createdChapter.Id }, createdChapter);
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
                return BadRequest(ModelState);
            }

            try
            {
                var chapter = await _chapterService.UpdateChapterAsync(id, chapterDTO);
                return Ok(chapter);
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
                return NotFound(ex.Message); // Return 404 if the chapter is not found
            }
        }

        //[Authorize(Policy = "Admin-Manager")]
        //[HttpDelete]
        //[Route("{id}")]
        //[SwaggerOperation(
        //    Summary = "Authorization: Admin & Content Manager",
        //    Description = "Delete a chapter"
        //)]
        //public async Task<IActionResult> DeleteChapter([Required] string id)
        //{
        //    try
        //    {
        //        var chapterAppService = _appChapterServices as IAppChapterServices;
        //        if (await chapterAppService.DeleteChapterAsync(id))
        //        {
        //            return Ok("Delete successfully");
        //        }
        //        return BadRequest("Delete unsuccessfully");
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound("Error: " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error: " + ex.Message);
        //    }
        //}


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
                return BadRequest("Search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                return BadRequest("Search term must be at least 2 characters long.");
            }

            try
            {
                var chapters = await _chapterService.SearchChapterAsync(searchTerm, pageNumber, pageSize);
                return Ok(chapters);
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

        // GET: api/ChapterAccess/{chapterId}
        [HttpGet("/ChapterAccess/{chapterId}")]
        public async Task<IActionResult> CanAccessChapter(string chapterId)
        {
            if (string.IsNullOrWhiteSpace(chapterId))
            {
                return BadRequest("Chapter Id is required.");
            }

            try
            {
                bool canAccess = await _chapterService.CanAccessChapterAsync(chapterId);
                string chapterName = await _chapterService.GetChapterNameAsync(chapterId);

                if (canAccess)
                {
                    return Ok(new { Message = $"You can access chapter '{chapterName}'." });
                }
                else
                {
                    return Forbid($"You cannot access chapter '{chapterName}' until the required quiz for the previous chapter is completed.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}