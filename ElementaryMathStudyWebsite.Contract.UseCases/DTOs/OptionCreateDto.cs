namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OptionCreateDto
    {
        public string QuestionId { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
    }
}
