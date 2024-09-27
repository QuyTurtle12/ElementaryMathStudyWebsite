namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizCreateDto
    {
        public string QuizName { get; set; } = string.Empty; 
        public double Criteria { get; set; }
        public string ChapterName { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
    }
}
