using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Services.Service;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.TopicPages
{
    public class DetailsModel : PageModel
    {
        private readonly IAppTopicServices _topicService;

        public DetailsModel(IAppTopicServices topicService)
        {
            _topicService = topicService ?? throw new ArgumentNullException(nameof(topicService));
        }

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
                Quiz = new Core.Repositories.Entity.Quiz // Assuming Quiz is a class with a property QuizName
                {
                    QuizName = topicDto.QuizName // Replace with the actual property name for the quiz name
                }
            };

            return Page();
        }
    }
}
