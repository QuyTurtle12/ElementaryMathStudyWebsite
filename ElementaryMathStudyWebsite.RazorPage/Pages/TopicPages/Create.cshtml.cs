using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Services.Service;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.TopicPages
{
    public class CreateModel : PageModel
    {
        private readonly IAppTopicServices _topicService;
        private readonly IAppUserServices _userService;

        public CreateModel(IAppTopicServices topicService, IAppUserServices userService)
        {
            _topicService = topicService ?? throw new ArgumentNullException(nameof(topicService));
            _userService = userService;
        }
        public List<string> ChapterNames { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ChapterId"] = new SelectList(await _topicService.GetChaptersAllAsync(), "Id", "ChapterName");
            
            ViewData["QuizId"] = new SelectList(await _topicService.GetQuizzesWithoutChapterOrTopicAsync(), "Id", "QuizName");
            return Page();
        }

        [BindProperty]
        public Topic Topic { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            string currentUserId = HttpContext.Session.GetString("user_id");
            User? currentUser = await _userService.GetUserByIdAsync(currentUserId);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if TopicName already exists
            bool topicExists = await _topicService.GetAllTopicsAsync(1, 100)
                .ContinueWith(t => t.Result.Items.Any(t => t.TopicName == Topic.TopicName));

            if (topicExists)
            {
                ModelState.AddModelError("Topic.TopicName", "TopicName này đã tồn tại.");
                return RedirectToPage("./Index");
            }

            // Create new TopicCreateDto
            var topicCreateAllDto = new TopicCreateAllDto
            {
                TopicName = Topic.TopicName,
                TopicContext = Topic.TopicContext,
                Number = Topic.Number,
                ChapterId = Topic.ChapterId,
                QuizId = Topic.QuizId,
                CreatedByUser = currentUserId,
                CreatedTime = DateTime.UtcNow
            };

            try
            {
                var createdTopic = await _topicService.AddTopicAllAsync(topicCreateAllDto);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra: " + ex.Message);
                return Page();
            }
        }
    }
}
