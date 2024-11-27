namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OptionUpdateDto
    {
        public string Id {  get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
    }
}
