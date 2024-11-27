using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.TopicPages
{
    public class DeleteModel : PageModel
    {
        private readonly IAppTopicServices _topicService;
        private readonly IAppUserServices _userService;

        public DeleteModel(IAppTopicServices topicService, IAppUserServices userService)
        {
            _topicService = topicService ?? throw new ArgumentNullException(nameof(topicService));
            _userService = userService;
        }

        [BindProperty]
        public Topic Topic { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var topicDto = await _topicService.GetTopicByIdAsync(id);
            if (topicDto == null)
            {
                return NotFound();
            }

            Topic = new Topic
            {
                Id = topicDto.Id,
                TopicName = topicDto.TopicName,
                Number = topicDto.Number ?? 0,
                TopicContext = topicDto.TopicContext,
                Chapter = new Chapter // Assuming Chapter is a class with a property ChapterName
                {
                    ChapterName = topicDto.ChapterName // Replace with the actual property name for the chapter name
                },
                Quiz = new Quiz // Assuming Quiz is a class with a property QuizName
                {
                    QuizName = topicDto.QuizName // Replace with the actual property name for the quiz name
                }
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            string currentUserId = HttpContext.Session.GetString("user_id");
            User? currentUser = await _userService.GetUserByIdAsync(currentUserId);

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var topicCreateAllDto = new TopicCreateAllDto
            {
                Id = id,
                
                LastUpdatedByUser = currentUserId,
                LastUpdatedTime = DateTime.Now,
                DeletedById = currentUserId,
                DeletedTime = DateTime.UtcNow
            };

            try
            {
                var deletedTopic = await _topicService.DeleteTopicRazorAsync(id, topicCreateAllDto);
                return RedirectToPage("./Index");
            }
            catch (Exception)
            {
                // Handle exception (e.g., log the error)
                return RedirectToPage("./Index"); // Redirect on failure as well, or show an error message
            }
        }
    }
}