using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using ElementaryMathStudyWebsite.Services.Service;


namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly IAppTopicServices _topicService;
        public TopicsController(IAppTopicServices topicService)
        {
            _topicService = topicService ?? throw new ArgumentNullException(nameof(topicService));
        }

        [HttpGet]
        [Route("admin-content/all")]
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Lấy danh sách Topic (Admin-Manager)"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<TopicAdminViewDto>>>> GetAllTopics(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                BasePaginatedList<TopicAdminViewDto>? topic = await _topicService.GetAllExistTopicsAsync(pageNumber, pageSize);
                var response = BaseResponse<BasePaginatedList<TopicAdminViewDto>>.OkResponse(topic);
                return response;
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

            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }

            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }

        [Authorize(Policy = "Admin-Content")]
        [HttpGet]
        [Route("admin-content/allDelete")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Lấy danh sách Topic đã delete"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<TopicAdminViewDto>>>> GetAllDeleteTopics(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                BasePaginatedList<TopicAdminViewDto>? topic = await _topicService.GetAllDeleteTopicsAsync(pageNumber, pageSize);
                var response = BaseResponse<BasePaginatedList<TopicAdminViewDto>>.OkResponse(topic);
                return response;
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

            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }

            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }

        [HttpGet]
        [Route("admin-content/{id}")]
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Lấy thông tin 1 Topic (Admin-Manager)"
            )]
        public async Task<ActionResult<TopicAdminViewDto>> GetAllTopicById([Required] string id)
        {
            //if (string.IsNullOrWhiteSpace(id))
            //{
            //    return BadRequest(new
            //    {
            //        errorCode = "InvalidId",
            //        errorMessage = "The provided ID is invalid."
            //    });
            //}

            try
            {
                var topic = await _topicService.GetTopicAllByIdAsync(id);
                var response = BaseResponse<object>.OkResponse(topic);
                return Ok(response);
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

            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }

            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }

        [HttpGet]
        [Route("user/all/")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Lấy danh sách Topics (User)"
            )]
        public async Task<IActionResult> GetAllTopicsForUsers(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _topicService.GetAllTopicsAsync(pageNumber, pageSize);
                return Ok(result);
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
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }

            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }

        [HttpGet]
        [Route("User/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Lấy Topic theo Id (User)"
            )]
        public async Task<ActionResult<TopicViewDto>> GetTopicById([Required] string id)
        {
            //if (string.IsNullOrWhiteSpace(id))
            //{
            //    return BadRequest(new
            //    {
            //        errorCode = "InvalidId",
            //        errorMessage = "The provided ID is invalid."
            //    });
            //}

            try
            {
                var topic = await _topicService.GetTopicByIdAsync(id);
                var response = BaseResponse<object>.OkResponse(topic);
                return Ok(response);
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
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }

            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}

        }

        [HttpGet("chapter/{chapterId}")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Lấy danh sách Topics bằng ChapterId"
            )]
        public async Task<ActionResult<List<TopicViewDto>>> GetTopicsByChapterId(string chapterId)
        {
            //if (string.IsNullOrWhiteSpace(chapterId))
            //{
            //    return BadRequest(new
            //    {
            //        errorCode = "InvalidId",
            //        errorMessage = "The provided ID is invalid."
            //    });
            //}

            try
            {
                var topic = await _topicService.GetTopicsByChapterIdAsync(chapterId);
                var response = BaseResponse<object>.OkResponse(topic);
                return Ok(response);
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
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }

            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }

        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Tìm kiếm Topic bằng Topic theo Name"
            )]
        public async Task<IActionResult> SearchTopicByName([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _topicService.SearchTopicByNameAsync(searchTerm, pageNumber, pageSize);
                return Ok(result);
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
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }

            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Create Topic"
            )]
        public async Task<IActionResult> AddTopic(TopicCreateDto topicCreateDto)
        {
            try
            {
                var addTopic = await _topicService.AddTopicAsync(new TopicCreateDto
                {
                    Number = topicCreateDto.Number,
                    TopicName = topicCreateDto.TopicName,
                    TopicContext = topicCreateDto.TopicContext,
                    QuizId = topicCreateDto.QuizId,
                    ChapterId = topicCreateDto.ChapterId,
                });
                //return Ok(addTopic);
                var response = BaseResponse<TopicAdminViewDto>.OkResponse(addTopic);
                return CreatedAtAction(nameof(GetTopicById), new { id = addTopic.Id }, response);
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
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }

            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }

        [HttpPut]
        [Route("update/{id}")]
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Update Topic"
            )]
        public async Task<IActionResult> UpdateTopic(string id, [FromBody] TopicUpdateDto topicUpdateDto)
        {
            try
            {
                var result = await _topicService.UpdateTopicAsync(id, new TopicUpdateDto
                {
                    TopicName = topicUpdateDto.TopicName,
                    TopicContext = topicUpdateDto.TopicContext,
                    QuizId = topicUpdateDto.QuizId,
                    ChapterId = topicUpdateDto.ChapterId,
                });

                var successResponse = BaseResponse<object>.OkResponse(result);
                return Ok(successResponse);
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
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle specific NotFoundException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }
            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }

        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Delete Topic"
            )]
        public async Task<IActionResult> DeleteTopic(string id)
        {
            //if (string.IsNullOrWhiteSpace(id))
            //{
            //    return BadRequest(new
            //    {
            //        errorCode = "InvalidId",
            //        errorMessage = "The provided ID is invalid."
            //    });
            //}
            try
            {
                var result = await _topicService.DeleteTopicAsync(id);

                if (result)
                {
                    var successResponse = BaseResponse<string>.OkResponse("Delete successfully");
                    return Ok(successResponse);

                }
                var failedResponse = BaseResponse<string>.OkResponse("Delete unsuccessfully");

                return Ok(failedResponse);
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
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }
        }

        [HttpPut]
        [Route("rollbakTopic/{id}")]
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Rollback topic was deleted"
        )]
        public async Task<IActionResult> RollBackTopicDeleted([Required] string id)
        {
            try
            {
                var topic = await _topicService.RollBackTopicDeletedAsync(id);
                var response = BaseResponse<object>.OkResponse(topic);
                return Ok(response);
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
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }

            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }

        [HttpPut]
        [Route("swap-numbers")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Hoán đổi số thứ tự của hai Topic"
        )]
        public async Task<IActionResult> SwapTopicNumbers([Required] string topicId1, [Required] string topicId2)
        {
            //if (string.IsNullOrWhiteSpace(topicId1) || string.IsNullOrWhiteSpace(topicId2))
            //{
            //    return BadRequest(new
            //    {
            //        errorCode = "InvalidIds",
            //        errorMessage = "Both topic IDs must be provided."
            //    });
            //}

            try
            {
                await _topicService.SwapTopicNumbersAsync(topicId1, topicId2);
                return Ok(new
                {
                    message = "Topic numbers swapped successfully."
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

            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }

            //catch (Exception ex)
            //{
            //    // Handle any other exceptions
            //    return StatusCode(500, new
            //    {
            //        errorCode = "InternalServerError",
            //        errorMessage = "An unexpected error occurred.",
            //        details = ex.Message
            //    });
            //}
        }
    }
}