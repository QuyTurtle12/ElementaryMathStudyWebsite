namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizViewDto
    {
        public string QuizName { get; set; } = string.Empty;
        public double? Criteria { get; set; }
        public String Status { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastUpdatedByUserId { get; set; }
        public int? DeletedByUserId { get; set; }
        public int? ChapterId { get; set; }
        public int? TopicId { get; set; }
        public ICollection<int>? QuestionIds { get; set; } // ID của các câu hỏi liên quan
        public ICollection<int>? ProgressIds { get; set; } // ID của các tiến trình liên quan

        public QuizViewDto(String quizName, int chapterId, int topicId) {
            QuizName = quizName;
            ChapterId = chapterId;
            TopicId = topicId;
        }

    }
}
