using Microsoft.AspNetCore.Mvc;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Services.Service;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using System.ComponentModel.DataAnnotations;


namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;
        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService ?? throw new ArgumentNullException(nameof(topicService));
        }

        [HttpGet]
        public async Task<ActionResult<BasePaginatedList<Topic>>> GetTopics(int pageNumber = -1, int pageSize = -1)
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Topic>> GetAllTopicById([Required] string id)
        {
            try
            {
                Topic topic = await _topicService.GetTopicAllByIdAsync(id);
                if (topic == null)
                {
                    return BadRequest("Invalid Order Id");
                }
                return Ok(topic);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpGet("{idd}")]
        public async Task<ActionResult<TopicViewDto>> GetTopicById([Required] string id)
        {
            try
            {
                var appService = _topicService as IAppTopicServices;
                TopicViewDto topic = await appService.GetTopicByIdAsync(id);
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

        [HttpPost]
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

        [HttpPut("{id}")]
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
    }
}