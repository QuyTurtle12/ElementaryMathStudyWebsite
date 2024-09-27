namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicDeleteDto : TopicAdminViewDto
    {
        public string DeletedBy { get; set; } = string.Empty;
        public string DeleteName { get; set; } = string.Empty;
        public string DeletePhone { get; set; } = string.Empty;
        public DateTimeOffset? DeletedTime { get; set; }
    }
}