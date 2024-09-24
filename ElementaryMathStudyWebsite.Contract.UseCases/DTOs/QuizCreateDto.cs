namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class CreateQuizDto
    {
        public string QuizName { get; set; } = string.Empty; 
        public double? Criteria { get; set; } 
    }
}
