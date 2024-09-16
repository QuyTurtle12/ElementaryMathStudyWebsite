namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicCreateDto
    {
        public int? Number { get; set; } // Number use for arranging the topic orderly

        public string TopicName { get; set; } = string.Empty; // avoid null reference issues

        public bool Status { get; set; } = true;

        public string? QuizId { get; set; }

        public required string ChapterId { get; set; }

        public string? CreatedByUser { get; set; }
        public string? LastUpdatedByUser { get; set; }
        public string? DeletedByUser { get; set; }

    }
}
