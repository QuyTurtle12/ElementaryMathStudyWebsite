namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicAdminViewDto
    {
        public string Id { get; set; } = string.Empty;
        public int? Number { get; set; } // Number use for arranging the topic orderly
        public string TopicName { get; set; } = string.Empty; // avoid null reference issues
        public string? TopicContext { get; set; }
        public bool Status { get; set; } = true;
        public string? QuizId { get; set; }
        public string? QuizName { get; set; }
        public string? ChapterId { get; set; }
        public string? ChapterName { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string CreatorPhone { get; set; } = string.Empty;
        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedPersonName { get; set; } = string.Empty;
        public string LastUpdatedPersonPhone { get; set; } = string.Empty;
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
    }
}