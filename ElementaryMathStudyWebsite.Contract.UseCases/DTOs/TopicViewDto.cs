using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicViewDto
    {
        public int? Number { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string QuizName { get; set; } = string.Empty;
        public string ChapterName { get; set; } = string.Empty;

        // Default constructor
        public TopicViewDto() { }

        // Constructor with parameters
        public TopicViewDto(int? number, string topicName, string quizName, string chapterName)
        {
            Number = number;
            TopicName = topicName;
            QuizName = quizName;
            ChapterName = chapterName;
        }
    }
}
