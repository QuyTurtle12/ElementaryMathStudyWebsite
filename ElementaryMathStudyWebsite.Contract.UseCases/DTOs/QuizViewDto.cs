namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizViewDto
    {

        //public string QuizId { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public double? Criteria { get; set; }
        public bool? Status { get; set; }
        public string? ChapterName { get; set; }
        public string? TopicName { get; set; }
    }
}
