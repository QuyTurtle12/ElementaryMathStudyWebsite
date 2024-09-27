using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Services.Service;
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
        public ChaptersController(IAppChapterServices chapterServices)
        {
            _chapterService = chapterServices;
        }
        // GET: api/chapters/Content
        // Get chapters for Content & Admin
        [Authorize(Policy = "Admin-Content")]
        [HttpGet]
        [Route("Content")]
        [SwaggerOperation(
            Summary = "Authorization: Content & Admin",
            Description = "View chapter list for Content and Admin Role. Insert -1 to get all items"
        )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterAdminViewDto?>>>> GetChapters(int pageNumber = -1, int pageSize = -1)
        {
            BasePaginatedList<ChapterAdminViewDto?> chapters = await _chapterService.GetChaptersAsync(pageNumber, pageSize);
            var response = BaseResponse<BasePaginatedList<ChapterAdminViewDto?>>.OkResponse(chapters);
            return response;
        }


        // GET: api/chapters/Content/{id}
        // Get chapters for Content & Admin
        [Authorize(Policy = "Admin-Content")]
        [HttpGet]
        [Route("Content/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Content & Admin",
            Description = "View chapter for Content and Admin Role."
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterAdminViewDto?>>>> GetChapter(string id)
        {
            var chapter = await _chapterService.GetChapterByChapterIdAsync(id);
            if (chapter == null)
            {
                throw new BaseException.BadRequestException("chapter_not_found", "The requested chapter was not found.");
            }

            var response = BaseResponse<object>.OkResponse(chapter);
            return Ok(response);
        }


        // GET: api/chapters/{id}
        // Get chapters for general user
        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "View chapter for General User"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterViewDto?>>>> GetChapterForGeneralUser([Required] string id)
        {
            var chapter = await _chapterService.GetChapterDtoByChapterIdAsync(id);
            if (chapter == null)
            {
                throw new BaseException.BadRequestException("chapter_not_found", "The requested chapter was not found.");
            }

            var response = BaseResponse<object>.OkResponse(chapter);
            return Ok(response);
        }


        // GET: api/chapter
        // Get chapters for general user
        //[HttpGet]
        //[SwaggerOperation(
        //    Summary = "Authorization: N/A",
        //    Description = "View chapter list for General User. Insert -1 to get all items"
        //    )]
        //public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterViewDto?>>>> GetChaptersForGeneralUser(int pageNumber = -1, int pageSize = -1)
        //{
        //        BasePaginatedList<ChapterViewDto?> chapters = await _chapterService.GetChapterDtosAsync(pageNumber, pageSize);
        //        var response = BaseResponse<BasePaginatedList<ChapterViewDto?>>.OkResponse(chapters);
        //        return response;
        //   
        //}

        [HttpGet]
        [Route("subject")]
        [SwaggerOperation(
           Summary = "Authorization: N/A",
           Description = "View all chapters of 1 subject"
           )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterViewDto>>>> GetChapterBySubjectId([Required] string subjectId, int pageNumber = -1, int pageSize = -1)
        {
            BasePaginatedList<ChapterViewDto> chapters = await _chapterService.GetChaptersBySubjectIdAsync(pageNumber, pageSize, subjectId);
            var response = BaseResponse<BasePaginatedList<ChapterViewDto>>.OkResponse(chapters);
            return response;
        }


        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Search chapter by name, pageSize = -1 to have it show all."
        )]
        public async Task<IActionResult> SearchChapter([FromQuery] string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var subjects = await _chapterService.SearchChapterAsync(searchTerm, pageNumber, pageSize);
            var response = BaseResponse<BasePaginatedList<object>>.OkResponse(subjects);
            return Ok(response);
        }

        [Authorize(Policy = "Admin-Content")]
        [HttpGet("search/admin")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Content",
            Description = "Search chapter by name for admin, pageSize = -1 to have it show all."
        )]
        public async Task<IActionResult> SearchChapterForAdmin([FromQuery] string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var subjects = await _chapterService.SearchChapterForAdminAsync(searchTerm, pageNumber, pageSize);
            if (subjects?.Items.Count == 0 || subjects == null)
            {
                throw new BaseException.NotFoundException("no_subjects_found", "No chapters match the search criteria.");
            }

            var response = BaseResponse<BasePaginatedList<object>>.OkResponse(subjects);
            return Ok(response);
        }

        // POST: api/chapters/
        // Add chapters
        [Authorize(Policy = "Admin-Content")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content",
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

            var createdChapter = await _chapterService.CreateChapterAsync(new ChapterDto
            {
                ChapterName = chapterDTO.ChapterName,
                //Status = true, // Set status as active when created
                SubjectId = chapterDTO.SubjectId,
                QuizId = chapterDTO.QuizId,
            });
            var response = BaseResponse<ChapterViewDto>.OkResponse(createdChapter);

            return CreatedAtAction(nameof(GetChapterForGeneralUser), new { id = createdChapter.Id }, response);

        }

        [Authorize(Policy = "Admin-Content")]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content",
            Description = "Update chapter"
        )]
        public async Task<IActionResult> UpdateChapter(string id, [FromBody] ChapterUpdateDto chapterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    error = "Invalid Model State",
                    details = ModelState
                });
            }

            var updatedChapter = await _chapterService.UpdateChapterAsync(id, chapterDTO);
            var response = BaseResponse<ChapterAdminViewDto>.OkResponse(updatedChapter);
            return Ok(response);

        }

        [Authorize(Policy = "Admin-Content")]
        [HttpPost("assign-quiz")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content",
            Description = "Assign quizId to chapter"
        )]
        public async Task<IActionResult> AssignQuizIdToChapter(string chapterId, string quizId)
        {
            var updatedChapter = await _chapterService.AssignQuizIdToChapterAsync(chapterId, quizId);
            var response = BaseResponse<bool>.OkResponse(updatedChapter);
            return Ok(response);


        }


        [Authorize(Policy = "Admin-Content")]
        [HttpPut("update-numbers")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content Manager",
            Description = "Update chapter number"
        )]
        public async Task<IActionResult> UpdateChapterNumbers(string subjectId, [FromBody] ChapterNumberDto chapterNumberDto)
        {
            if (string.IsNullOrEmpty(subjectId) || chapterNumberDto == null || !chapterNumberDto.ChapterNumbersOrder.Any())
            {
                return BadRequest(new { message = "Invalid input data." });
            }

            var updatedChapterNumber = await _chapterService.UpdateChapterNumbersAsync(subjectId, chapterNumberDto);
            var response = BaseResponse<bool>.OkResponse(updatedChapterNumber);
            return Ok(response);

        }

        [Authorize(Policy = "Admin-Content")]
        [HttpPut("/StatusChange/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin, Content",
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
            var chapter = await _chapterService.ChangeChapterStatusAsync(id);
            if (chapter == null)
            {
                throw new BaseException.NotFoundException("chapter_not_found", "The requested chapter was not found.");
            }
            var response = BaseResponse<ChapterAdminViewDto>.OkResponse(chapter);
            return Ok(response);

        }


        [Authorize(Policy = "Admin-Content")]
        [HttpPut("/rollbackChapter/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content",
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

            var chapter = await _chapterService.rollbackChapterDeletedAsync(id);
            if (chapter == null)
            {
                throw new BaseException.NotFoundException("chapter_not_found", "The requested chapter was not found.");
            }
            var response = BaseResponse<ChapterAdminViewDto>.OkResponse(chapter);
            return Ok(response);

        }


        [Authorize(Policy = "Admin-Content")]
        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content",
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
            var result = await _chapterService.DeleteChapterAsync(id);

            if (result)
            {
                var successResponse = BaseResponse<string>.OkResponse("Delete successfully");
                return Ok(successResponse);

            }
            var failedResponse = BaseResponse<string>.OkResponse("Delete unsuccessfully");

            return Ok(failedResponse);

        }


        [Authorize(Policy = "Admin-Content")]
        [HttpGet]
        [Route("Content/deleted")]
        [SwaggerOperation(
            Summary = "Authorization: Content & Admin",
            Description = "View list chapter was deleted for Content and Admin Role. Insert -1 to get all items"
        )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterAdminViewDto>>>> GetChaptersDeleted(int pageNumber = -1, int pageSize = -1)
        {
            BasePaginatedList<ChapterAdminViewDto> chapters = await _chapterService.GetChaptersDeletedAsync(pageNumber, pageSize);
            var response = BaseResponse<BasePaginatedList<ChapterAdminViewDto>>.OkResponse(chapters);
            return response;

        }

        //// GET: api/ChapterAccess/{chapterId}
        //[HttpGet("/ChapterAccess/{chapterId}")]
        //public async Task<ActionResult<BaseResponse<object>>> CanAccessChapter(string chapterId)
        //{
        //    try
        //    {
        //        // Call the service method to check if the student can access the chapter
        //        bool canAccess = await _chapterService.CanAccessChapterAsync(chapterId);
        //        string chapterName = await _chapterService.GetChapterNameAsync(chapterId);

        //        if (canAccess)
        //        {
        //            // Return a successful response using BaseResponse
        //            return Ok(BaseResponse<object>.OkResponse($"You can access chapter '{chapterName}'."));
        //        }
        //        else
        //        {
        //            // Return a forbidden response using BaseResponse
        //            return StatusCode(StatusCodes.Status403Forbidden, new BaseResponse<object>(
        //                StatusCodeHelper.BadRequest,
        //                "Forbbiden",
        //                $"You cannot access chapter '{chapterName}' until the required quiz for the previous chapter is completed."
        //            ));
        //        }
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Handle KeyNotFoundException by returning a 404 response using BaseResponse
        //        return NotFound(new BaseResponse<object>(
        //            StatusCodeHelper.BadRequest,
        //            "Not Found",
        //            ex.Message
        //        ));
        //    }
        //    catch (BaseException.CoreException coreEx)
        //    {
        //        // Handle specific CoreException
        //        var errorResponse = new
        //        {
        //            code = coreEx.Code,
        //            message = coreEx.Message,
        //            additionalData = coreEx.AdditionalData
        //        };
        //        return StatusCode(coreEx.StatusCode, new BaseResponse<object>(
        //            StatusCodeHelper.BadRequest,
        //            coreEx.Code,
        //            errorResponse
        //        ));
        //    }
        //    catch (BaseException.BadRequestException badRequestEx)
        //    {
        //        // Handle specific BadRequestException
        //        return BadRequest(new BaseResponse<object>(
        //            StatusCodeHelper.BadRequest,
        //            badRequestEx.ErrorDetail.ErrorCode,
        //            badRequestEx.ErrorDetail.ErrorMessage
        //        ));
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any other exceptions with a 500 response using BaseResponse
        //        var errorResponse = new
        //        {
        //            Message = "An error occurred.",
        //            Details = ex.Message
        //        };
        //        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse<object>(
        //            StatusCodeHelper.ServerError,
        //            "Server Error",
        //            errorResponse
        //        ));
        //    }
        //}

    }
}