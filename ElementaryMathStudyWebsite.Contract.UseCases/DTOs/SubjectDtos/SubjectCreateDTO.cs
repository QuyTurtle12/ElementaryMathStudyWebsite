namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos
{
    public class SubjectCreateDTO
    {
        public string SubjectName { get; set; } = string.Empty; // avoid null reference issues

        public double Price { get; set; } = 0;
    }
}
