namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicViewDto
    {
        public string Id { get; set; } = string.Empty;
        public int? Number { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string? TopicContext { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public string ChapterName { get; set; } = string.Empty;

    }
}
