using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;


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

        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("admin-manager/all")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "Lấy danh sách Topic (Admin-Manager)"
            )]
        public async Task<ActionResult<BasePaginatedList<TopicAdminViewDto>>> GetTopics(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                BasePaginatedList<TopicAdminViewDto> topics = await _topicService.GetAllExistTopicsAsync(pageNumber, pageSize);
                return Ok(topics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Invalid input: " + ex.Message);
            }
        }

        [Authorize(Policy = "Admin-Manager")]
        [HttpGet]
        [Route("admin-manager/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Manager & Admin",
            Description = "Lấy thông tin 1 Topic (Admin-Manager)"
            )]
        public async Task<ActionResult<TopicAdminViewDto>> GetAllTopicById([Required] string id)
        {
            try
            {
                TopicAdminViewDto? topic = await _topicService.GetTopicAllByIdAsync(id);
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
        [Route("user/all/")]
        public async Task<IActionResult> GetAllTopics(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }

            var result = await _topicService.GetAllTopicsAsync(pageNumber, pageSize);
            return Ok(result);
        }
    

        [HttpGet]
        [Route("User/{id}")]
        public async Task<ActionResult<TopicViewDto>> GetTopicById([Required] string id)
        {
            try
            {
                TopicViewDto? topic = await _topicService.GetTopicByIdAsync(id);
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

        [HttpGet("chapter/{chapterId}")]
        public async Task<ActionResult<List<TopicViewDto>>> GetTopicsByChapterId(string chapterId)
        {
            try
            {
                var topics = await _topicService.GetTopicsByChapterIdAsync(chapterId);
                return Ok(topics);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                // Log the exception (not shown here)
                return StatusCode(500, "Internal server error");
            }
        }

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
        public async Task<ActionResult<string>> UpdateTopic(string id, [FromBody] TopicCreateDto topicCreateDto)
        {
            try
            {
                var appService = _topicService as IAppTopicServices;
                bool isUpdated = await appService.UpdateTopicAsync(id, topicCreateDto);
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

        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Policy = "Admin-Manager")]
        [SwaggerOperation(
            Summary = "Authorization: Admin-Manager",
            Description = "Delete Topic"
            )]
        public async Task<IActionResult> DeleteTopic(string id)
        {
            try
            {
                var deletedTopicDto = await _topicService.DeleteTopicAsync(id);
                return Ok(deletedTopicDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                // Log the exception (not shown here)
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}