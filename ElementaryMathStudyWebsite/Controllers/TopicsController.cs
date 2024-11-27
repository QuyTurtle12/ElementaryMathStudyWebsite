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
        [HttpPut]
        [Route("update-all/{id}")]
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Cập nhật thông tin đầy đủ cho một chủ đề"
        )]
        public async Task<IActionResult> UpdateTopicAll(string id, [FromBody] TopicCreateAllDto topicCreateAllDto)
        {
            if (topicCreateAllDto == null)
            {
                return BadRequest("Thông tin chủ đề không hợp lệ.");
            }

            try
            {
                var updatedTopic = await _topicService.UpdateTopicAllAsync(id, topicCreateAllDto);
                var successResponse = BaseResponse<TopicAdminViewDto>.OkResponse(updatedTopic);
                return Ok(successResponse);
            }
            catch (BaseException.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BaseException.BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        // GET: api/chapter/all
        [HttpGet("allchapter")]
        public async Task<ActionResult<List<ChapterDto>>> GetChaptersAll()
        {
            var chapters = await _topicService.GetChaptersAllAsync();
            if (chapters == null || chapters.Count == 0)
            {
                return NotFound("No chapters found.");
            }

            return Ok(chapters);
        }

        [HttpGet("chapter/name/{chapterName}")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Lấy danh sách Topics theo tên chương"
        )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<TopicViewDto>>>> GetTopicsByChapterNameAsync(string chapterName, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(chapterName))
            {
                return BadRequest("Tên chương không được để trống.");
            }

            var topics = await _topicService.GetTopicsByChapterNameAsync(chapterName, pageNumber, pageSize);
            var response = BaseResponse<BasePaginatedList<TopicViewDto>>.OkResponse(topics);
            return Ok(response);
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
            BasePaginatedList<TopicAdminViewDto>? topic = await _topicService.GetAllExistTopicsAsync(pageNumber, pageSize);
            var response = BaseResponse<BasePaginatedList<TopicAdminViewDto>>.OkResponse(topic);
            return response;
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
            BasePaginatedList<TopicAdminViewDto>? topic = await _topicService.GetAllDeleteTopicsAsync(pageNumber, pageSize);
            var response = BaseResponse<BasePaginatedList<TopicAdminViewDto>>.OkResponse(topic);
            return response;
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
            var topic = await _topicService.GetTopicAllByIdAsync(id);
            var response = BaseResponse<object>.OkResponse(topic);
            return Ok(response);
        }

        [HttpGet]
        [Route("user/all/")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Lấy danh sách Topics (User)"
            )]
        public async Task<IActionResult> GetAllTopicsForUsers(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _topicService.GetAllTopicsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("User/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Lấy Topic theo Id (User)"
            )]
        public async Task<ActionResult<TopicViewDto>> GetTopicById([Required] string id)
        {
            var topic = await _topicService.GetTopicByIdAsync(id);
            var response = BaseResponse<object>.OkResponse(topic);
            return Ok(response);

        }

        [HttpGet("chapter/{chapterId}")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Lấy danh sách Topics bằng ChapterId"
            )]
        public async Task<ActionResult<List<TopicViewDto>>> GetTopicsByChapterId(string chapterId, int pageNumber = 1, int pageSize = 10)
        {
            var topic = await _topicService.GetTopicsByChapterIdAsync(chapterId, pageNumber, pageSize);
            var response = BaseResponse<object>.OkResponse(topic);
            return Ok(response);
        }

        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "Tìm kiếm Topic bằng Topic theo Name"
            )]
        public async Task<IActionResult> SearchTopicByName([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _topicService.SearchTopicByNameAsync(searchTerm, pageNumber, pageSize);
            return Ok(result);

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

        [HttpPut]
        [Route("update/{id}")]
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Update Topic"
            )]
        public async Task<IActionResult> UpdateTopic(string id, [FromBody] TopicUpdateDto topicUpdateDto)
        {
            var result = await _topicService.UpdateTopicAsync(id, new TopicUpdateDto
            {
                TopicName = topicUpdateDto.TopicName,
                TopicContext = topicUpdateDto.TopicContext,
            });

            var successResponse = BaseResponse<object>.OkResponse(result);
            return Ok(successResponse);
        }

        [HttpPut]
        [Route("updateQuizId/{id}")]
        [Authorize(Policy = "Admin-Content")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Update QuizId Topic"
            )]
        public async Task<IActionResult> UpdateQuizIdTopic(string id, [FromBody] TopicUpdateQuizIdDto topicUpdateDto)
        {
            var result = await _topicService.UpdateQuizIdTopicAsync(id, new TopicUpdateQuizIdDto
            {
                QuizId = topicUpdateDto.QuizId,
            });

            var successResponse = BaseResponse<object>.OkResponse(result);
            return Ok(successResponse);
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
            var deletedTopic = await _topicService.DeleteTopicAsync(id);
            var successResponse = BaseResponse<object>.OkResponse(deletedTopic);
            return Ok(successResponse);
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
            var topic = await _topicService.RollBackTopicDeletedAsync(id);
            var response = BaseResponse<object>.OkResponse(topic);
            return Ok(response);
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
            await _topicService.SwapTopicNumbersAsync(topicId1, topicId2);
            return Ok(new
            {
                message = "Topic numbers swapped successfully."
            });
        }
    }
}