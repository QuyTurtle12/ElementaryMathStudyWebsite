﻿namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class CartCreateDto
    {
        public IEnumerable<SubjectStudentDto> SubjectStudents { get; set; } = new List<SubjectStudentDto>();

    }

    public class SubjectStudentDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;

    }
}