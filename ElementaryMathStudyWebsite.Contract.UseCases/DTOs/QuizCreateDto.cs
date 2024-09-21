namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizCreateDto
    {
        public string QuizName { get; set; }
        public double? Criteria { get; set; }
        public string ChapterId { get; set; }
        public string TopicId { get; set; }
    }
}
