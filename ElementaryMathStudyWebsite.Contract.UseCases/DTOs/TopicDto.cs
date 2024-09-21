namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicDto
    {
        public int Number { get; set; }

        public string TopicName { get; set; }
        public bool Status { get; set; }

        public string? QuizId { get; set; }

        public required string ChapterId { get; set; }
    }
}
