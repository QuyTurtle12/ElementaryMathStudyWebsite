﻿namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ProgressViewDto
    {
        public string StudentId { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public string SubjectId { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public double SubjectPercentage { get; set; } = 0;

        public IEnumerable<FinishedTopic>? FinishedTopics {  get; set; } = new List<FinishedTopic>();
        
        public IEnumerable<FinishedChapter>? FinishedChapters { get; set; } = new List<FinishedChapter>();
    }

    public class FinishedTopic
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class FinishedChapter
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
