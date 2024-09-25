namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos
{
    public class SubjectDTO : ISubjectBaseDTO
    {
        public required string Id { get; set; }
        public string SubjectName { get; set; } = string.Empty; // avoid null reference issues

        public double Price { get; set; }

        public bool Status { get; set; }
    }
}
