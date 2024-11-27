using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;


namespace ElementaryMathStudyWebsite.RazorPage.Pages.TopicPages
{
    public class EditModel : PageModel
    {
        private readonly IAppTopicServices _topicService;
        private readonly IAppUserServices _userService;

        public EditModel(IAppTopicServices topicService, IAppUserServices userService)
        {
            _topicService = topicService ?? throw new ArgumentNullException(nameof(topicService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [BindProperty]
        public Topic Topic { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID không hợp lệ.");
            }

            try
            {
                var topicDto = await _topicService.GetTopicByIdAsync(id);
                if (topicDto == null)
                {
                    return NotFound(); // Return 404 if topic is not found
                }

                // Map DTO to Entity
                Topic = new Topic
                {
                    Id = topicDto.Id,
                    TopicName = topicDto.TopicName,
                    TopicContext = topicDto.TopicContext,
                    Number = topicDto.Number ?? 0,
                    ChapterId = topicDto.ChapterId,
                    QuizId = topicDto.QuizId,
                };

                ViewData["ChapterId"] = new SelectList(await _topicService.GetChaptersAllAsync(), "Id", "ChapterName");
                ViewData["QuizId"] = new SelectList(await _topicService.GetQuizzesWithoutChapterOrTopicAsync(), "Id", "QuizName");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra: " + ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            string currentUserId = HttpContext.Session.GetString("user_id");
            User? currentUser = await _userService.GetUserByIdAsync(currentUserId);

            if (!ModelState.IsValid)
            {
                // If the model state is invalid, reload the necessary data for the dropdowns
                ViewData["ChapterId"] = new SelectList(await _topicService.GetChaptersAllAsync(), "Id", "ChapterName");
                ViewData["QuizId"] = new SelectList(await _topicService.GetQuizzesWithoutChapterOrTopicAsync(), "Id", "QuizName");
                return Page();
            }

            try
            {
                // Prepare the DTO for the update
                var topicCreateAllDto = new TopicCreateAllDto
                {
                    Id = id,
                    Number = Topic.Number,
                    TopicName = Topic.TopicName,
                    TopicContext = Topic.TopicContext,
                    ChapterId = Topic.ChapterId,
                    QuizId = null,
                    LastUpdatedByUser = currentUserId,
                };

                // Call the update service method
                var updatedTopic = await _topicService.UpdateTopicAllAsync(id, topicCreateAllDto);

                // Optionally, you can add a success message or redirect to a different page
                TempData["SuccessMessage"] = "Cập nhật chủ đề thành công!";
                return RedirectToPage("./Index"); // Redirect to the index page after update
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra: " + ex.Message);
                // Reload the dropdown values on error
                ViewData["ChapterId"] = new SelectList(await _topicService.GetChaptersAllAsync(), "Id", "ChapterName");
                ViewData["QuizId"] = new SelectList(await _topicService.GetQuizzesWithoutChapterOrTopicAsync(), "Id", "QuizName");
                return Page();
            }
        }
    }
}