namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizUpdateDto
    {
        public string QuizName { get; set; } = string.Empty; 
        public double Criteria { get; set; }
        public bool Status { get; set; }
    }
}