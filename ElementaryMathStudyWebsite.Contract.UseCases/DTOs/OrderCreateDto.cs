namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OrderCreateDto
    {
        public List<SubjectStudentDto> SubjectStudents { get; set; } = new List<SubjectStudentDto>();

        public OrderCreateDto() { }

        public OrderCreateDto(List<SubjectStudentDto> subjectStudents)
        {
            SubjectStudents = subjectStudents;
        }
    }

    public class SubjectStudentDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;

        public SubjectStudentDto() { }

        public SubjectStudentDto(string subjectId, string studentId)
        {
            SubjectId = subjectId;
            StudentId = studentId;
        }
    }
}
