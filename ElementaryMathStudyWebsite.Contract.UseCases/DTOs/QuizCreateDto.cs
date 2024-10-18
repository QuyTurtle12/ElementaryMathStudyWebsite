namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizCreateDto
    {
        public string QuizName { get; set; } = string.Empty;
        public double Criteria { get; set; }
    }
}
