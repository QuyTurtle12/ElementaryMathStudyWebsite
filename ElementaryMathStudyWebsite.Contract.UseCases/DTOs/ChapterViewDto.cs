namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ChapterViewDto
    {
        public required string Id { get; set; }
        public int Number { get; set; } = 0;
        public string ChapterName { get; set; } = string.Empty;
        public bool Status { get; set; }

        public required string SubjectId { get; set; }

        public string SubjectName { get; set; } = string.Empty ;

        public string? QuizId { get; set; } // Quiz may not exist yet and can be created later

        public string QuizName { get; set; } = string.Empty;

    }
}