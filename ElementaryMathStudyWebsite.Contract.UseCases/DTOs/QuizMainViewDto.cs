namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizMainViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string QuizName { get; set; } = string.Empty;
        public double? Criteria { get; set; }
        public bool? Status { get; set; }

        public string ChapterName { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string CreatorPhone { get; set; } = string.Empty;
        public DateTimeOffset CreatedTime { get; set; }

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedPersonName { get; set; } = string.Empty;
        public string LastUpdatedPersonPhone { get; set; } = string.Empty;
        public DateTimeOffset LastUpdatedTime { get; set; }
    }
}