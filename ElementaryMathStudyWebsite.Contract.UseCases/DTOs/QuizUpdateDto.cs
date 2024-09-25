namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizUpdateDto
    {
        public string QuizName { get; set; } = string.Empty; 
        public double Criteria { get; set; }
        public bool Status { get; set; } 
        public string ChapterId { get; set; } = string.Empty; 
        public string TopicId { get; set; } = string.Empty; 
    }
}