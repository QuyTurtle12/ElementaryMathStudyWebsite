namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OrderDetailViewDto
    {
        public string SubjectName { get; set; }

        public string StudentName { get; set; }

        public OrderDetailViewDto() { }

        public OrderDetailViewDto(string subjectName, string studentName)
        {
            SubjectName = subjectName;
            StudentName = studentName;
        }
    }
}
