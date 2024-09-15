using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizDetailsDto
    {
        public string QuizName { get; set; } = string.Empty;
        public double? Criteria { get; set; }
        public ChapterDto? Chapter { get; set; }
        public TopicDto? Topic { get; set; }  
        public IList<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
    public class ChapterDto
    {
        public string ChapterName { get; set; } = string.Empty;
    }

    public class TopicDto
    {
        public string TopicName { get; set; } = string.Empty;
    }

    public class QuestionDto
    {
        public string QuestionContext { get; set; } = string.Empty;
    }
}
