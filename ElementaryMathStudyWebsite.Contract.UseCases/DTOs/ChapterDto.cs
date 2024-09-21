namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ChapterDto
    {
        public int? Number { get; set; }
        public string ChapterName { get; set; } = string.Empty;
        public bool Status { get; set; }

        public required string SubjectId { get; set; }

        public string? QuizId { get; set; } // Quiz may not exist yet and can be created later

        //public string? CreatedBy { get; set; }
        //public string? LastUpdatedBy { get; set; }
    }
}