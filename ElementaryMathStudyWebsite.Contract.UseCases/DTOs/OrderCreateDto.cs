﻿namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OrderCreateDto
    {
        public string CustomerId { get; set; } = string.Empty;
        public List<SubjectStudentDto> SubjectStudents { get; set; } = new List<SubjectStudentDto>();

        public OrderCreateDto() { }

        public OrderCreateDto(string customerId, List<SubjectStudentDto> subjectStudents)
        {
            CustomerId = customerId;
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