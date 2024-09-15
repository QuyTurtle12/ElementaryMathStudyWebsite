namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ProgressViewDto
    {
        public string StudentName { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public double SubjectPercentage { get; set; }

        public ProgressViewDto() { }

        public ProgressViewDto(string studentName, string subjectName, double subjectPercentage)
        {
            StudentName = studentName;
            SubjectName = subjectName;
            SubjectPercentage = subjectPercentage;
        }
    }
}
