namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ChapterAdminDelete :ChapterAdminViewDto
    {

        public string? DeletedBy { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public string DeleteChapterBy { get; set; } = string.Empty;
    }
}