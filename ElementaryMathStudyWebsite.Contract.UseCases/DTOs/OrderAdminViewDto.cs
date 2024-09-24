namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OrderAdminViewDto : OrderViewDto
    {
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string CreatorPhone { get; set; } = string.Empty;
        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedPersonName { get; set; } = string.Empty;
        public string LastUpdatedPersonPhone { get; set; } = string.Empty;
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; } 
    }
}