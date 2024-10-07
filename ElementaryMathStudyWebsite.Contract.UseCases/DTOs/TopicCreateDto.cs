
namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicCreateDto
    {
        public int Number { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string? TopicContext { get; set; }
        public string? QuizId { get; set; }
        public string ChapterId { get; set; } = string.Empty;
        //public string CreatedByUser { get; set; } = string.Empty;
        //public string LastUpdatedByUser { get; set; } = string.Empty;
        //public string DeletedById { get; set; } = string.Empty;
        //public string LastDeletedBy { get; set; } = string.Empty;
        //public DateTimeOffset? DeletedTime { get; set; }
        //public DateTimeOffset LastUpdatedTime { get; set; }
        //public DateTimeOffset CreatedTime { get; set; }
    }
}