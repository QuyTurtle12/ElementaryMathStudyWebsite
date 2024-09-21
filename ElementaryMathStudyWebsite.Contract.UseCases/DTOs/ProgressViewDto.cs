namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ProgressViewDto
    {
        public string StudentId { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public string SubjectId { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public double SubjectPercentage { get; set; }

    }
}
