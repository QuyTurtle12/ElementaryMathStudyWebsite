namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ChapterAdminViewDto
    {
        public required string Id { get; set; }
        public int? Number { get; set; }
        public string ChapterName { get; set; } = string.Empty;
        public bool Status { get; set; }

        public required string SubjectId { get; set; }

        public string? QuizId { get; set; } // Quiz may not exist yet and can be created later

        public string? CreatedBy { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }

    }
}