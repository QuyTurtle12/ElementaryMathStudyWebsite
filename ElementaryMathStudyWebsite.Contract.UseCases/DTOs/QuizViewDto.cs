namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizViewDto
    {
        public QuizViewDto(string quizName, double? criteria, bool? status)
        {
            QuizName = quizName;
            Criteria = criteria;
            Status = status;
        }

        public QuizViewDto()
        {
        }

        //public string QuizId { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public double? Criteria { get; set; }
        public bool? Status { get; set; }
        public string? ChapterName { get; set; }
        public string? TopicName { get; set; }
    }
}
