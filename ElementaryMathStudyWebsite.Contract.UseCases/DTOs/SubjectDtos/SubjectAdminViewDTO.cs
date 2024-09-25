namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos
{
    public class SubjectAdminViewDTO : ISubjectBaseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public double Price { get; set; } = 0;
        public bool Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
