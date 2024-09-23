namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicDto
    {
        public string? Id { get; set; }
        public int Number { get; set; }

        public string? TopicName { get; set; }
        public bool Status { get; set; }

        public string? QuizId { get; set; }

        public string? ChapterId { get; set; }
    }
}
