using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Services.Service;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.TopicPages
{
    public class IndexModel : PageModel
    {
        private readonly IAppTopicServices _topicService;
        private readonly IAppChapterServices _chapterService;
        private readonly IAppUserServices _userService;

        public IndexModel(IAppTopicServices topicService, IAppChapterServices chapterService, IAppUserServices userService)
        {
            _topicService = topicService ?? throw new ArgumentNullException(nameof(topicService));
            _chapterService = chapterService ?? throw new ArgumentNullException(nameof(chapterService));
            _userService = userService;
        }
        public string UserRole { get; private set; } = string.Empty;
        public BasePaginatedList<TopicViewDto>? Topics { get; set; } = default!;
        public List<string> ChapterNames { get; set; } = new List<string>(); // Danh sách tên chương
        public string? ChapterId { get; set; } // Thêm ChapterId

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1, int pageSize = 10, string searchString = null, string chapterName = null, string chapterId = null)
        {
            string currentUserId = HttpContext.Session.GetString("user_id")!;
            UserRole = HttpContext.Session.GetString("role_name");

            ChapterNames = await _topicService.GetChapterNamesAsync();
            ChapterId = chapterId;

            // Lọc theo chapterId nếu có
            if (!string.IsNullOrEmpty(chapterId))
            {
                Topics = await _topicService.GetTopicsByChapterIdAsync(chapterId, pageNumber, pageSize);
            }

            // Lọc theo tên chương nếu có
            else if (!string.IsNullOrEmpty(chapterName))
            {
                Topics = await _topicService.GetTopicsByChapterNameAsync(chapterName, pageNumber, pageSize);
            }
            // Nếu có searchString, tìm kiếm theo tên topic
            else if (!string.IsNullOrEmpty(searchString))
            {
                Topics = await _topicService.SearchTopicByNameAsync(searchString, pageNumber, pageSize);
            }
            else
            {
                Topics = await _topicService.GetAllTopicsAsync(pageNumber, pageSize);
            }


            return Page();
        }
    }
}