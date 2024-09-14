using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly IChapterService _chapterService;

        public ChaptersController(IChapterService chapterService)
        {
            _chapterService = chapterService ?? throw new ArgumentNullException(nameof(chapterService));
        }

        // GET: api/chapters/manager
        // Get chapters for Manager & Admin
        //[Authorize(Policy = "Admin-Manager")]
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
        //[Authorize(Policy = "Admin-Manager")]
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
                // Cast domain service to application service
                var appService = _chapterService as IAppChapterServices;

                ChapterViewDto chapter = await appService.GetChapterDtoByChapterIdAsync(id);
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
                // Cast domain service to application service
                var appService = _chapterService as IAppChapterServices;

                BasePaginatedList<ChapterViewDto> chapters = await appService.GetChapterDtosAsync(pageNumber, pageSize);
                return Ok(chapters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        // POST: api/chapters/
        // Add chapters
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Parent",
            Description = "Create chapter."
        )]
        public async Task<ActionResult<string>> AddChapter(ChapterCreateDto chapterCreateDto)
        {
            try
            {
                // Cast domain service to application service
                var chapterAppService = _chapterService as IAppChapterServices;

                // Add new chapter
                bool isAddedNewChapter = await chapterAppService.AddChapterAsync(chapterCreateDto);

                if (!isAddedNewChapter)
                {
                    return BadRequest("Failed to create chapter, please check input value");
                }

                return Ok("Created Chapter Successfully!");
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error occurred while creating chapter");
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

    }
}