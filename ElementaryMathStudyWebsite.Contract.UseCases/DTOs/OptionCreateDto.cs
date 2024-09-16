namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OptionCreateDto
    {
        public required string QuestionId { get; set; }
        public string? Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
