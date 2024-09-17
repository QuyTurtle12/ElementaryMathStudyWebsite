namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OrderDetailViewDto
    {
        public OrderDetailViewDto(string subjectName, string? studentName)
        {
            SubjectName = subjectName;
            StudentName = studentName;
        }

        public string SubjectName { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

    }
}
