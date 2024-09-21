namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class CreateQuestionDto
    {
        public required string QuestionContext { get; set; }
        public required string QuizId { get; set; }
    }
}
