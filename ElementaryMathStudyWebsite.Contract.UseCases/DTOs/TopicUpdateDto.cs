namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicUpdateDto
    {
        public string TopicName { get; set; } = string.Empty;
        public string? TopicContext { get; set; }
    }
}
