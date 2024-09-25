namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class CartCreateDto
    {
<<<<<<< HEAD
        public IEnumerable<SubjectStudentDto> SubjectStudents { get; set; } = [];

=======
        public IEnumerable<SubjectStudentDto> SubjectStudents { get; set; } = new List<SubjectStudentDto>();
>>>>>>> 979c02fba4e0b01837805fe6c340895673dad8b9
    }

    public class SubjectStudentDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;

    }
}