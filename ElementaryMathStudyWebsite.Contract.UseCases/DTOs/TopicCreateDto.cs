
namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicCreateDto
    {
        public int? Number { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string? TopicContext { get; set; }
        public bool Status { get; set; } = true;
        public string? QuizId { get; set; }
        public required string ChapterId { get; set; }
        public string CreatedByUser { get; set; } = string.Empty;
        public string LastUpdatedByUser { get; set; } = string.Empty;
        public string DeletedById { get; set; } = string.Empty;
        public string LastDeletedBy { get; set; } = string.Empty;
        public DateTimeOffset? DeletedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
    }
}
