namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizDetailsDto
    {
        public string QuizName { get; set; } = string.Empty;
        public double? Criteria { get; set; }
        public ChapterDetailDto? Chapter { get; set; }
        public TopicDetailDto? Topic { get; set; }  
        public IList<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
    public class ChapterDetailDto
    {
        public string ChapterName { get; set; } = string.Empty;
    }

    public class TopicDetailDto
    {
        public string TopicName { get; set; } = string.Empty;
    }

    public class QuestionDto
    {
        public string QuestionContext { get; set; } = string.Empty;
    }
}
