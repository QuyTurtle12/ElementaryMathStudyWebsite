namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuestionViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string QuestionContext { get; set; } = string.Empty;
        public required string QuizName { get; set; }
    }
}