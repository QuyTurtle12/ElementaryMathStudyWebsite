namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ChapterAdminViewDto : ChapterViewDto
    {
        public string? CreatedBy { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public string CreatorPhone { get; set; } = string.Empty;
        public DateTimeOffset? CreatedTime { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string LastUpdatedPersonName { get; set; } = string.Empty;
        public string LastUpdatedPersonPhone { get; set; } = string.Empty;
        public DateTimeOffset? LastUpdatedTime { get; set; }
        //public string? DeletedBy { get; set; }
        //public DateTimeOffset? DeletedTime { get; set; }

    }
}