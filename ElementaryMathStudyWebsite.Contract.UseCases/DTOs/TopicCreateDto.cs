using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class TopicCreateDto
    {
        public int? Number { get; set; } // Number use for arranging the topic orderly

        public string TopicName { get; set; } = string.Empty; // avoid null reference issues

        public bool Status { get; set; }

        public string? QuizId { get; set; }

        public required string ChapterId { get; set; }

        public string? CreatedByUser { get; set; }
        public string? LastUpdatedByUser { get; set; }
        public string? DeletedByUser { get; set; }

        public TopicCreateDto() { }

        public TopicCreateDto(int? number, string topicName, string chapterId, string? quizId)
        {
            Number = number;
            TopicName = topicName;
            Status = true; // Always true when initialized
            ChapterId = chapterId;
            QuizId = quizId;
        }
    }
}
