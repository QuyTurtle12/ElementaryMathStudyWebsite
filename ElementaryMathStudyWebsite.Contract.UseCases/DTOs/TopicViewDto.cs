namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicViewDto
    {
        public int? Number { get; set; }
        public string TopicName { get; set; } = string.Empty;

        public string QuizName { get; set; } = string.Empty;
        public string ChapterName { get; set; } = string.Empty;

    }
}
