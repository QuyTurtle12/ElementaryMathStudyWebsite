﻿namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OrderCreateDto
    {
        public IEnumerable<SubjectStudentDto> SubjectStudents { get; set; } = new List<SubjectStudentDto>();
        public string OrderId { get; set; }
    }

    public class SubjectStudentDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;

    }
}
