namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuestionViewDto
    {
        public string Id { get; set; } = string.Empty; // Ensure this is a GUID
        public string QuestionContext { get; set; } = string.Empty;
        public string QuizId { get; set; } = string.Empty;

    }
}