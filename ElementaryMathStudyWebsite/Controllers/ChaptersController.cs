using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;

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
        // GET: api/chapters/manager
        // Get chapters for Manager & Admin
        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("manager")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "View chapter list for Manager and Admin Role. Insert -1 to get all items"
        )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterAdminViewDto?>>>> GetChapters(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                BasePaginatedList<ChapterAdminViewDto?> chapters = await _chapterService.GetChaptersAsync(pageNumber, pageSize);
                var response = BaseResponse<BasePaginatedList<ChapterAdminViewDto?>>.OkResponse(chapters);
                return response;
            }
            //catch (BaseException.CoreException coreEx)
            //{
            //    // Handle specific CoreException
            //    return StatusCode(coreEx.StatusCode, new
            //    {
            //        code = coreEx.Code,
            //        message = coreEx.Message,
            //        additionalData = coreEx.AdditionalData
            //    });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
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
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterAdminViewDto?>>>> GetChapter(string id)
        {
            try
            {
                    var chapter = await _chapterService.GetChapterByChapterIdAsync(id);
                    if (chapter == null)
                    {
                        throw new BaseException.BadRequestException("chapter_not_found", "The requested chapter was not found.");
                    }

                    var response = BaseResponse<object>.OkResponse(chapter);
                    return Ok(response);
                    //var chapter = await _chapterService.GetChapterByChapterIdAsync(id);
                    //if (chapter == null)
                    //{
                    //    return NotFound(new
                    //    {
                    //        error = "Invalid Chapter Id",
                    //        message = "The chapter with the specified ID was not found."
                    //    });
                    //}
                    //return Ok(chapter);
            }
            //catch (BaseException.CoreException coreEx)
            //{
            //    // Handle specific CoreException
            //    return StatusCode(coreEx.StatusCode, new
            //    {
            //        code = coreEx.Code,
            //        message = coreEx.Message,
            //        additionalData = coreEx.AdditionalData
            //    });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
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
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterViewDto?>>>> GetChapterForGeneralUser([Required] string id)
        {
            try
            {
                var chapter = await _chapterService.GetChapterDtoByChapterIdAsync(id);
                if (chapter == null)
                {
                    throw new BaseException.BadRequestException("chapter_not_found", "The requested chapter was not found.");
                }

                var response = BaseResponse<object>.OkResponse(chapter);
                return Ok(response);
            }

            //catch (BaseException.CoreException coreEx)
            //{
            //    // Handle specific CoreException
            //    return StatusCode(coreEx.StatusCode, new
            //    {
            //        code = coreEx.Code,
            //        message = coreEx.Message,
            //        additionalData = coreEx.AdditionalData
            //    });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
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
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterViewDto?>>>> GetChaptersForGeneralUser(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                BasePaginatedList<ChapterViewDto?> chapters = await _chapterService.GetChapterDtosAsync(pageNumber, pageSize);
                var response = BaseResponse<BasePaginatedList<ChapterViewDto?>>.OkResponse(chapters);
                return response;
            }
            //catch (BaseException.CoreException coreEx)
            //{
            //    // Handle specific CoreException
            //    return StatusCode(coreEx.StatusCode, new
            //    {
            //        code = coreEx.Code,
            //        message = coreEx.Message,
            //        additionalData = coreEx.AdditionalData
            //    });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        error = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }

        //[HttpGet]
        //[Route("subject")]
        //[SwaggerOperation(
        //   Summary = "Authorization: N/A",
        //   Description = "View all chapters of 1 subject"
        //   )]
        //public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterViewDto>>>> GetChapterBySubjectId([Required] string subjectId, int pageNumber = -1, int pageSize = -1)
        //{
        //    try
        //    {
        //        BasePaginatedList<ChapterViewDto> chapters = await _chapterService.GetChaptersBySubjectIdAsync(pageNumber, pageSize, subjectId);
        //        var response = BaseResponse<BasePaginatedList<ChapterViewDto>>.OkResponse(chapters);
        //        return response;
        //    }
        //    //catch (BaseException.CoreException coreEx)
        //    //{
        //    //    // Handle specific CoreException
        //    //    return StatusCode(coreEx.StatusCode, new
        //    //    {
        //    //        code = coreEx.Code,
        //    //        message = coreEx.Message,
        //    //        additionalData = coreEx.AdditionalData
        //    //    });
        //    //}
        //    catch (BaseException.BadRequestException badRequestEx)
        //    {
        //        // Handle specific BadRequestException
        //        return BadRequest(new
        //        {
        //            errorCode = badRequestEx.ErrorDetail.ErrorCode,
        //            errorMessage = badRequestEx.ErrorDetail.ErrorMessage
        //        });
        //    }
        //    //catch (Exception ex)
        //    //{
        //    //    // Handle any other exceptions
        //    //    return StatusCode(500, new
        //    //    {
        //    //        error = "An unexpected error occurred.",
        //    //        details = ex.Message
        //    //    });
        //    //}
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
                var chapter = await _chapterService.SearchChapterAsync(searchTerm, pageNumber, pageSize);
                if (chapter == null)
                {
                    throw new BaseException.BadRequestException("chapter_not_found", "The requested chapter was not found.");
                }

                var response = BaseResponse<object>.OkResponse(chapter);
                return Ok(response);
            }
            //catch (KeyNotFoundException ex)
            //{
            //    // Handle case when no chapters are found
            //    return NotFound(new
            //    {
            //        error = "Not Found",
            //        message = ex.Message
            //    });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            //catch (Exception ex)
            //{
            //    // Handle unexpected errors
            //    return StatusCode(500, new
            //    {
            //        error = "Internal Server Error",
            //        message = ex.Message
            //    });
            //}
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
                var chapter = await _chapterService.SearchChapterForAdminAsync(searchTerm, pageNumber, pageSize);
                if (chapter == null)
                {
                    throw new BaseException.BadRequestException("chapter_not_found", "The requested chapter was not found.");
                }

                var response = BaseResponse<object>.OkResponse(chapter);
                return Ok(response);
            }
            //catch (KeyNotFoundException ex)
            //{
            //    // Handle case when no chapters are found
            //    return NotFound(new
            //    {
            //        error = "Not Found",
            //        message = ex.Message
            //    });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            //catch (Exception ex)
            //{
            //    // Handle unexpected errors
            //    return StatusCode(500, new
            //    {
            //        error = "Internal Server Error",
            //        message = ex.Message
            //    });
            //}
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
                var response = BaseResponse<ChapterAdminViewDto>.OkResponse(createdChapter);

                return CreatedAtAction(nameof(GetChapterForGeneralUser), new { id = createdChapter.Id }, response);
            }
            //catch (ArgumentException argEx)
            //{
            //    // Handle argument exceptions such as validation errors
            //    return StatusCode(400, new
            //    {
            //        errorMessage = "An unexpected error occurred.",
            //        details = argEx.Message
            //    });
            //}
            //catch (BaseException.CoreException coreEx)
            //{
            //    // Handle specific CoreException
            //    return StatusCode(coreEx.StatusCode, new
            //    {
            //        code = coreEx.Code,
            //        message = coreEx.Message,
            //        additionalData = coreEx.AdditionalData
            //    });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            //catch (Exception ex)
            //{
            //    // Catch all other exceptions and return a generic server error
            //    return StatusCode(500, new
            //    {
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
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
                var updatedChapter = await _chapterService.UpdateChapterAsync(id, chapterDTO);
                var response = BaseResponse<ChapterAdminViewDto>.OkResponse(updatedChapter);
                return Ok(response);
            }
            //catch (ArgumentException argEx)
            //{
            //    // Handle argument exceptions such as validation errors
            //    return StatusCode(400, new
            //    {
            //        errorMessage = "An unexpected error occurred.",
            //        details = argEx.Message
            //    });
            //}
            //catch (BaseException.CoreException coreEx)
            //{
            //    // Handle specific CoreException
            //    return StatusCode(coreEx.StatusCode, new
            //    {
            //        code = coreEx.Code,
            //        message = coreEx.Message,
            //        additionalData = coreEx.AdditionalData
            //    });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            //catch (Exception ex)
            //{
            //    // Catch all other exceptions and return a generic server error
            //    return StatusCode(500, new
            //    {
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
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
                if (chapter == null)
                {
                    throw new BaseException.BadRequestException("chapter_not_found", "The requested chapter was not found.");
                }
                var response = BaseResponse<ChapterAdminViewDto>.OkResponse(chapter);
                return Ok(response);
            }
            //catch (BaseException.CoreException coreEx)
            //{
            //    return StatusCode(coreEx.StatusCode, new { code = coreEx.Code, message = coreEx.Message, additionalData = coreEx.AdditionalData });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                return BadRequest(new { errorCode = badRequestEx.ErrorDetail.ErrorCode, errorMessage = badRequestEx.ErrorDetail.ErrorMessage });
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
                if (chapter == null)
                {
                    throw new BaseException.BadRequestException("chapter_not_found", "The requested chapter was not found.");
                }
                var response = BaseResponse<ChapterAdminViewDto>.OkResponse(chapter);
                return Ok(response);
            }
            //catch (KeyNotFoundException ex)
            //{
            //    return NotFound(new
            //    {
            //        error = "Not Found",
            //        message = ex.Message
            //    });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            //catch (Exception ex)
            //{
            //    // Handle unexpected errors
            //    return StatusCode(500, new
            //    {
            //        error = "Internal Server Error",
            //        message = ex.Message
            //    });
            //}
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
                if (chapter == null)
                {
                    throw new BaseException.BadRequestException("chapter_not_found", "The requested chapter was not found.");
                }
                var response = BaseResponse<ChapterAdminViewDto>.OkResponse(chapter);
                return Ok(response);
            }
            //catch (KeyNotFoundException ex)
            //{
            //    return NotFound(new
            //    {
            //        error = "Not Found",
            //        message = ex.Message
            //    });
            //}
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            //catch (Exception ex)
            //{
            //    Handle unexpected errors
            //    return StatusCode(500, new
            //    {
            //        error = "Internal Server Error",
            //        message = ex.Message
            //    });
            //}
        }


        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("manager/deleted")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "View list chapter was deleted for Manager and Admin Role. Insert -1 to get all items"
        )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<ChapterAdminDelete?>>>> GetChaptersDeleted(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                BasePaginatedList<ChapterAdminDelete?> chapters = await _chapterService.GetChaptersDeletedAsync(pageNumber, pageSize);
                var response = BaseResponse<BasePaginatedList<ChapterAdminDelete?>>.OkResponse(chapters);
                return response;
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
            //catch (Exception ex)
            //{
            //    // Handle unexpected errors
            //    return StatusCode(500, new
            //    {
            //        error = "Internal Server Error",
            //        message = ex.Message
            //    });
            //}
        }

        // GET: api/ChapterAccess/{chapterId}
        [HttpGet("/ChapterAccess/{chapterId}")]
        public async Task<ActionResult<BaseResponse<object>>> CanAccessChapter(string chapterId)
        {
            try
            {
                // Call the service method to check if the student can access the chapter
                bool canAccess = await _chapterService.CanAccessChapterAsync(chapterId);
                string chapterName = await _chapterService.GetChapterNameAsync(chapterId);

                if (canAccess)
                {
                    // Return a successful response using BaseResponse
                    return Ok(BaseResponse<object>.OkResponse($"You can access chapter '{chapterName}'."));
                }
                else
                {
                    // Return a forbidden response using BaseResponse
                    return StatusCode(StatusCodes.Status403Forbidden, new BaseResponse<object>(
                        StatusCodeHelper.BadRequest,
                        "Forbbiden",
                        $"You cannot access chapter '{chapterName}' until the required quiz for the previous chapter is completed."
                    ));
                }
            }
            catch (KeyNotFoundException ex)
            {
                // Handle KeyNotFoundException by returning a 404 response using BaseResponse
                return NotFound(new BaseResponse<object>(
                    StatusCodeHelper.BadRequest,
                    "Not Found",
                    ex.Message
                ));
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                var errorResponse = new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                };
                return StatusCode(coreEx.StatusCode, new BaseResponse<object>(
                    StatusCodeHelper.BadRequest,
                    coreEx.Code,
                    errorResponse
                ));
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new BaseResponse<object>(
                    StatusCodeHelper.BadRequest,
                    badRequestEx.ErrorDetail.ErrorCode,
                    badRequestEx.ErrorDetail.ErrorMessage
                ));
            }
            catch (Exception ex)
            {
                // Handle any other exceptions with a 500 response using BaseResponse
                var errorResponse = new
                {
                    Message = "An error occurred.",
                    Details = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse<object>(
                    StatusCodeHelper.ServerError,
                    "Server Error",
                    errorResponse
                ));
            }
        }

    }
}