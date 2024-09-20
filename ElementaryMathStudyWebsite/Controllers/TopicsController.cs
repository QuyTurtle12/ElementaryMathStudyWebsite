﻿using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using ElementaryMathStudyWebsite.Core.Store;
using ElementaryMathStudyWebsite.Core.Utils;


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
        public async Task<ActionResult<BasePaginatedList<Topic>>> GetTopics(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                BasePaginatedList<Topic> topics = await _topicService.GetAllExistTopicsAsync(pageNumber, pageSize);
                return Ok(topics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Invalid input: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("all/{id}")]
        public async Task<ActionResult<Topic>> GetAllTopicById([Required] string id)
        {
            try
            {
                Topic topic = await _topicService.GetTopicAllByIdAsync(id);
                if (topic == null)
                {
                    return BadRequest("Invalid Id");
                }
                return Ok(topic);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<TopicViewDto>> GetTopicById([Required] string id)
        {
            try
            {
                TopicViewDto topic = await _topicService.GetTopicByIdAsync(id);
                if (topic == null)
                {
                    return NotFound("Topic not found.");
                }
                return Ok(topic);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        //[HttpGet("chapter/{chapterId}")]
        //public async Task<ActionResult<BasePaginatedList<TopicViewDto>>> GetTopicsByChapterIdAsync(string chapterId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(chapterId))
        //        {
        //            return BadRequest("Chapter ID cannot be empty.");
        //        }

        //        var result = await _topicService.GetTopicsByChapterIdAsync(chapterId, pageNumber, pageSize);

        //        if (result == null || result.Items.Count == 0)
        //        {
        //            return NotFound("No topics found for the specified chapter.");
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (use your logging framework)
        //        // For example: _logger.LogError(ex, "Error occurred while retrieving topics by chapter ID");

        //        return StatusCode(500, "An unexpected error occurred. Please try again later.");
        //    }
        //}

        [HttpGet("search")]
        public async Task<IActionResult> SearchTopicByName([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _topicService.SearchTopicByNameAsync(searchTerm, pageNumber, pageSize);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khác nếu cần
                return StatusCode(500, "Internal server error. " + ex.Message);
            }
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Create Topic"
            )]
        public async Task<ActionResult<string>> AddTopic(TopicCreateDto topicCreateDto)
        {
            try
            {
                var appService = _topicService as IAppTopicServices;
                bool isAddedNewTopic = await appService.AddTopicAsync(topicCreateDto);
                if (!isAddedNewTopic)
                {
                    return BadRequest("Failed to create topic, please check input values.");
                }
                return Ok("Created Chapter Successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("update/{id}")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Update Topic"
            )]
        public async Task<ActionResult<string>> UpdateTopic(string id, [FromBody] TopicDto topicDto)
        {
            try
            {
                var appService = _topicService as IAppTopicServices;
                bool isUpdated = await appService.UpdateTopicAsync(id, topicDto);
                if (!isUpdated)
                {
                    return NotFound("Topic not found.");
                }
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        // GET: api/TopicAccess/{topicId}/CanAccess
        [HttpGet("/TopicAccess/{topicId}")]
        public async Task<ActionResult<BaseResponse<object>>> CanAccessTopic(string topicId)
        {
            try
            {
                // Call the service method to check if the student can access the topic
                bool canAccess = await _topicService.CanAccessTopicAsync(topicId);
                string topicName = await _topicService.GetTopicNameAsync(topicId);

                if (canAccess)
                {
                    // Return a successful response using BaseResponse
                    return Ok(BaseResponse<object>.OkResponse($"You can access topic '{topicName}'."));
                }
                else
                {
                    // Return a forbidden response using BaseResponse
                    return StatusCode(StatusCodes.Status403Forbidden, new BaseResponse<object>(
                        StatusCodeHelper.BadRequest,
                        "Forbbiden",
                        $"You cannot access topic '{topicName}' until the required quiz for the previous topic is completed."
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